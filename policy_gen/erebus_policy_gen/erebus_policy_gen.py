'''
Erebus policy generator from natural language.
Trains a custom NER model for named entity recognition for policy statements in 
natural language.

Uses a combination of custom NER tagging and rule based matching to increase
accuracy of the conversion.
'''



import spacy
from spacy import displacy
from spacy.tokens import DocBin
from tqdm import tqdm
import json
import os
import re
from datetime import datetime


class nlp_model():

    def load_data(self):
        file = open('annotations.json')
        TRAIN_DATA = json.load(file)
        return TRAIN_DATA

    # spacy preprocess code to convert json into .spacy docbin
    def spacy_preprocess_data(self,TRAIN_DATA):

        nlp = spacy.blank('en') # load a new spacy model
        db = DocBin() # DocBin object to store the dataset
        for text, annotations in tqdm(TRAIN_DATA['annotations']):
            doc = nlp.make_doc(text)
            ents = []
            for start, end, label in annotations['entities']:
                span = doc.char_span(start, end, label=label)
                if span is None:
                    print("Skipping entity")
                else:
                    ents.append(span)
            doc.ents = ents
            db.add(doc)
        db.to_disk("./training_data.spacy")


    # use spacy's cli method for tranining 
    def spacy_train_model(self):
        os.system("python -m spacy init config config.cfg --lang en --pipeline \
            ner --optimize efficiency --force")
        os.system("python -m spacy train config.cfg --output ./ --paths.train \
            ./training_data.spacy --paths.dev ./training_data.spacy")


    def test_model(self, text=None):
        nlp_ner = spacy.load("model-best")
        if (text is None):
            text = "If the user is at home then allow the app to detect faces only during the evening"
        doc = nlp_ner(text)
        spacy.displacy.render(doc, style="ent", jupyter=True)
        print('Tokens', [(t.text, t.ent_type_, t.ent_iob) for t in doc])
        print('Entities', [(ent.text, ent.label_) for ent in doc.ents])


    def init_model(self):
        TRAIN_DATA = self.load_data()
        self.spacy_preprocess_data(TRAIN_DATA)
        self.spacy_train_model()
        #text = "Allow plane detection only within Stony Brook"
        #test_model(text)


    def get_ner_doc(self,text):
        if text is None:
            raise ValueError("Input text cannot be None")
        nlp_ner = spacy.load("erebus_policy_gen/model-best")
        doc = nlp_ner(text)
        return doc

    def get_entities(self,doc):
        if doc is None:
            raise ValueError("Doc object is null")
        
        action_ents = [ent for ent in doc.ents if ent.label_ == "ACTION"] or None
        location_ents = [ent for ent in doc.ents if ent.label_ == "LOCATION"] or None
        time_ents = [ent for ent in doc.ents if ent.label_ == "TIME"] or None
        user_ents = [ent for ent in doc.ents if ent.label_ == "USER"] or None
        username_ents = [ent for ent in doc.ents if ent.label_ == "USERNAME"] or None
        objname_ents = [ent for ent in doc.ents if ent.label_ == "OBJECTNAME"] or None
        #resource_ents = [ent for ent in doc.ents if ent.label_ == "RESOURCE"]
        trackables = ["PLANE DETECTION", "IMAGE TRACKING", "OBJECT TRACKING", 
                        "FACE TRACKING"]
        trackables_ents = [ent.label_ for ent in doc.ents if ent.label_ in trackables] or None
        if trackables_ents is None and location_ents is not None:
            trackables_ents = ["LOCATION TRACKING"]
        
        negate = [ent.label_ for ent in doc.ents if ent.label_ == "NEGATE"] or None
        entities = (trackables_ents, action_ents, location_ents, time_ents, user_ents, \
                    username_ents, objname_ents, negate)
        
        return entities
               

# file handling helper functions

class code_gen():

    FILE_NAME = "resources/input.el"
    __template_depth = 0

    def write_to_file(self,text=None):
        if text is None:
            return
        with open(self.FILE_NAME, 'a') as file:
            file.write(text)
            file.write("\n")

    def inc_template_depth(self):
        self.__template_depth += 1

    def dec_template_depth(self):
        self.__template_depth -= 1
        if self.__template_depth < 0:
            self.__template_depth = 0

    def add_template_depth(self):
        str = ""
        for i in range(0, self.__template_depth):
            str += "\t"
        return str

    def location_rules(self, location):
        if "home" in location.lower():
            return "Home"
        if "work" in location.lower():
            return "Work"
        return location

    def action_rules(self, action, negate):
        if "allow" in action.lower():
            if negate is not None:
                return "Deny"
            return "Allow"

        if "deny" in action.lower():
            if negate is not None:
                return "Allow"
            return "Deny"
        return "Deny"


    def timeconvert(self, str1):
        in_time = datetime.strptime(str1, "%I:%M%p")
        out_time = datetime.strftime(in_time, "%H%M")
        return out_time


    def create_function_body(self, entities):
        
        action, location, time, user, username, objname, negate = entities
        body_ = ""

        # initialize variables based on entities
        if location is not None:
            body_ += self.add_template_depth() + "let curLoc = GetCurrentLocation();\n"
            body_ += self.add_template_depth() + 'let trustedLoc = GetTrustedLocation("' \
                            + self.location_rules(location[0].text) +'");\n'
        
        if time is not None:
            body_ += self.add_template_depth() + "let curTime = GetCurrentTime();\n"
            for t in time:
                if "evening" in t.text.lower():
                    body_ += self.add_template_depth() + 'let validHour = GetValidHour("Evening");\n'
                    break
                if "morning" in t.text.lower():
                    body_ += self.add_template_depth() + 'let validHour = GetValidHour("Morning");\n'
                    break
                if "night" in t.text.lower():
                    body_ += self.add_template_depth() + 'let validHour = GetValidHour("Night");\n'
                    break
                if "weekdays" in t.text.lower():
                    body_ += self.add_template_depth() + 'let validHour = GetValidHour("Weekdays");\n'
                    break
                if "weekends" in t.text.lower():
                    body_ += self.add_template_depth() + 'let validHour = GetValidHour("Weekends");\n'
                    break
        
        if user is not None or username is not None:
            body_ += self.add_template_depth() + "let curFace = GetCurrentFaceId();\n"
            body_ += self.add_template_depth() + "let trustedFaces = GetTrustedFaceId("
            if username is not None:
                body_ += '"' + username[0].text + '"'
            else:
                body_ += '"Owner"'
            body_ += ");\n"
        
        if objname is not None:
            body_ += self.add_template_depth() + "let currentCameraFrame = GetCurrentCameraFrame();\n"    
            body_ += self.add_template_depth() + "let objName = " + '"' + objname[0].text + '";\n'
        

        # create the if condition based on entities

        if location is not None:
            body_ += self.add_template_depth() + "if ( "
            body_ += "curLoc.within(trustedLoc) )\n"
            body_ += self.add_template_depth() + "{\n"
            self.inc_template_depth()

        if time is not None:
            body_ += self.add_template_depth() + "if ( "
            if "after" in time[0].text.lower():
                pattern = r"(?:1[012]|[1-9])(?::[0-5][0-9])(?:am|pm|AM|PM)"
                result = re.findall(pattern, time[0].text)
                body_ += 'curTime > ' + self.timeconvert(result[0])
                body_ += " ) \n"
            elif "before" in time[0].text.lower():
                pattern = r"(?:1[012]|[1-9])(?::[0-5][0-9])(?:am|pm|AM|PM)"
                result = re.findall(pattern, time[0].text)
                body_ += 'curTime < ' + self.timeconvert(result[0])
                body_ += " ) \n"
            else:
                body_ += "curTime.within(validHour) )\n"
            
            body_ += self.add_template_depth() + "{\n"
            self.inc_template_depth()

        if user is not None or username is not None:
            body_ += self.add_template_depth() + "if ( "
            body_ += "curFace.matches(trustedFaces) )\n"
            body_ += self.add_template_depth() + "{\n"
            self.inc_template_depth()
        
        if objname is not None:
            body_ += self.add_template_depth() + "if ( "
            body_ += "currentCameraFrame.includes(objName) )\n"
            body_ += self.add_template_depth() + "{\n"
            self.inc_template_depth()


        # create the body of if --> Allow/Deny
        # body_ += self.add_template_depth() + "{\n"
        # inc_template_depth()
        body_ += self.add_template_depth() + self.action_rules(action[0].text, negate) + ";\n"

        while (self.__template_depth > 0):
            self.dec_template_depth()
            body_ += self.add_template_depth() + "}\n"
            
        return body_



    # convert trackable entities to code

    def plane_detection_api(self, entities):
        function_list = ["Raycast", "GetPlane", "ARPlaneTrackables", 
                        "RegisterEventOnPlanesChange", "UnRegisterEventOnPlanesChange"]
        
        fn_body = ""
        for fn in function_list:
            fn_body += "function " + fn + "()\n{\n"
            self.inc_template_depth()
            fn_body += self.create_function_body(entities)
            
        return fn_body        

    def image_detection_api(self,entities):
        function_list = ["RegisterEventOnImageChange",
                            "UnRegisterEventOnImageChange",
                            "AddTrackedImage"]
        
        fn_body = ""
        for fn in function_list:
            fn_body += "function " + fn + "()\n{\n"
            self.inc_template_depth()
            fn_body += self.create_function_body(entities)
            
        return fn_body        

    def object_detection_api(self,entities):
        function_list = ["GetObjectRawPixels"]
        
        fn_body = ""
        for fn in function_list:
            fn_body += "function " + fn + "()\n{\n"
            self.inc_template_depth()
            fn_body += self.create_function_body(entities)
            
        return fn_body        

    def location_detection_api(self,entities):
        function_list = ["GetGPSLocation"]
        
        fn_body = ""
        for fn in function_list:
            fn_body += "function " + fn + "()\n{\n"
            self.inc_template_depth()
            fn_body += self.create_function_body(entities)
            
        return fn_body        


    def generate_policy_from_text(self,text):
        if text is None:
            raise ValueError("Input text cannot be None")
        doc = nlp_model().get_ner_doc(text)
        #spacy.displacy.render(doc, style="ent", jupyter=True)
        entities = nlp_model().get_entities(doc)
        print(entities)
        output = ""
        for trackables in entities[0]:
            if trackables == "LOCATION TRACKING":
                output += self.location_detection_api(entities[1:])
            if trackables == "PLANE DETECTION":
                output += self.plane_detection_api(entities[1:])
            if trackables == "IMAGE TRACKING":
                output += self.image_detection_api(entities[1:])
            if trackables == "OBJECT TRACKING":
                output += self.object_detection_api(entities[1:])
        
        return(output)

def get_policy(text=None):
    if text is None:
        return None
    policy_code = code_gen().generate_policy_from_text(text)
    return policy_code



if __name__ == "__main__":
    try:
        #nlp_model().init_model()
        text = "If Batman is playing this game at Home allow plane detection"
        output = get_policy(text)
        #code_gen().write_to_file(output)
        print(output)
    except Exception as e:
        raise e


