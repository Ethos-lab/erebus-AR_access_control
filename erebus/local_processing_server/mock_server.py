from datetime import datetime
import random
import zmq
import msgpack
import time
from PIL import Image
import numpy as np
import json
import gzip
import cv2
import time

context = zmq.Context()
socket = context.socket(zmq.REP)
# socket.setsockopt(zmq.RCVTIMEO, 30000)
socket.bind("tcp://*:12345")

def covert_to_img_obj(raw_img_bytes):
    jpg_img = cv2.imdecode(np.asarray(bytearray(raw_img_bytes), dtype = "uint8"), cv2.IMREAD_COLOR)
    jpg_img = cv2.flip(jpg_img, 0)
    # cv2.imwrite(r"C:\Users\YoonsangKim\Documents\result.jpg", jpg_img)
    return jpg_img

def object_detection_func(jpg_img):
    np_data = np.array([
            # [0, "Table", 0.51, (0, 10, 250, 40), np.full((640, 480), False)],
            # [4, "Laptop", 0.12, (50, 100, 150, 150), np.full((640, 480), False)],
            [1, "Keyboard", 0.9, (50, 100, 150, 150), np.full((640, 480), False)],
            [2, "Laptop", 0.12, (50, 100, 150, 150), np.full((640, 480), False)],
            [2, "Laptop", 0.12, (50, 100, 150, 150), np.full((640, 480), False)],
            [2, "Laptop", 0.12, (50, 100, 150, 150), np.full((640, 480), False)],
            [2, "Laptop", 0.12, (50, 100, 150, 150), np.full((640, 480), False)],
            [2, "Laptop", 0.12, (50, 100, 150, 150), np.full((640, 480), False)],
            # [6, "Keyboard", 0.9, (200, 200, 100, 100), np.full((640, 480), False)],
            # [4, "Laptop", 0.12, (50, 100, 150, 150), np.full((640, 480), False)],
            # [5, "Keyboard", 0.9, (0, 0, 400, 400), np.full((640, 480), False)],
            # [7, "Laptop", 0.12, (50, 100, 150, 150), np.full((640, 480), False)],
            # [9, "Laptop", 0.12, (250, 350, 100, 100), np.full((640, 480), False)],
            [13, "TV", 0.92, (0, 0, 50, 50), np.full((640, 480), False)]], dtype = object)

    dataWrapper = {}
    data = []
    for index, np_elem in enumerate(np_data):
        elem = {}
        elem['Id'] = np_elem[0]
        elem['Label'] = np_elem[1]
        elem['Score'] = np_elem[2]
        elem['Bbox'] = np_elem[3]
        elem['Mask'] = np_elem[4].flatten()
        # min_index = random.randint(5000,10000)
        # max_index = random.randint(10000,100000)
        min_index = 0
        max_index = int(307200/2)
        elem['Mask'][min_index:max_index] = True
        min_index = int((307200 / 2))+ int((307200 / 4))
        max_index = int((307200 / 2)) + int((307200 / 2))
        elem['Mask'][min_index:max_index] = True
        elem['Mask'] = elem['Mask'].tolist()
        data.append(elem)

    dataWrapper['Data'] = data
    return_result = msgpack.packb(dataWrapper)
    # return_result = json.dumps(dataWrapper).encode('utf-8')

    # path = r'C:\Users\YoonsangKim\Desktop\TestProject\Assets\Resources\data.txt'
    # with open(path, "wb") as file:
    #     # Write bytes to file
    #     file.write(return_result)

    # decoded_data = json.loads(return_result)
    #
    # for elem in decoded_data:
    #     print("[%d,%s,%f,%s,%s]" %(elem['id'],
    #                             elem['label'],
    #                             elem['score'],
    #                             elem['bbox'],
    #                             elem['masks']))

    return return_result

# start_time = time.time()
# return_result = object_detection_func(None)
# end_time = time.time()
# print(end_time-start_time)


# compressed_result = gzip.compress(return_result, compresslevel=1)
# print(type(compressed_result))
# print("(len : %d)" % len(compressed_result))
# print("(len : %d)" % len(return_result))

print("SERVER RUNNING...")
cnt = 1
while True:
    try:
        raw_img_bytes = socket.recv()
        print("RECV MESSAGE %d (len : %d)" % (cnt, len(raw_img_bytes)))

        jpg_img = covert_to_img_obj(raw_img_bytes)
        send_data = object_detection_func(jpg_img)

        compressed_send_data = gzip.compress(send_data, compresslevel = 1)
        socket.send(compressed_send_data)

        print("SEND MESSAGE %d (len : %d [%d])" %(cnt, len(send_data), len(compressed_send_data)))
        print("\n___________")
        cnt += 1
    except zmq.error.Again as err_msg:
        print("SERVER TIMEOUT")
#         break
