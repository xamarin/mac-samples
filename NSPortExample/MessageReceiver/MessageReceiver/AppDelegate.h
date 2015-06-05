//
//  AppDelegate.h
//  MessageReceiver
//
//  Created by Chris Hamons on 6/4/15.
//  Copyright (c) 2015 Chris Hamons. All rights reserved.
//

#import <Cocoa/Cocoa.h>

@interface AppDelegate : NSObject <NSApplicationDelegate, NSTableViewDataSource, NSTableViewDelegate>

@property (weak) IBOutlet NSTableView *TheTable;
- (void) addItem:(NSString *)string;

@end
