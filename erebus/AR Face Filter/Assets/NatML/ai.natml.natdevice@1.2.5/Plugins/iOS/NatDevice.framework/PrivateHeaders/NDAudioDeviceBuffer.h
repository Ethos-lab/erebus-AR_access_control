//
//  NDAudioDeviceBuffer.h
//  NatDevice
//
//  Created by Yusuf Olokoba on 10/30/2021.
//  Copyright © 2022 NatML Inc. All rights reserved.
//

@import AVFoundation;

@interface NDAudioDeviceBuffer : NSObject
@property (readonly, nonnull) AVAudioPCMBuffer* buffer;
@property (readonly) UInt64 timestamp;
- (nonnull instancetype) initWithBuffer:(AVAudioPCMBuffer* _Nonnull) buffer andTimestamp:(UInt64) timestamp;
@end
