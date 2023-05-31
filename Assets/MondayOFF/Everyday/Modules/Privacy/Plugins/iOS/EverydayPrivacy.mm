//
//  EverydayPrivacy.mm
//
//  Created by heejong cho on 2/5/21.
//

#import "EverydayPrivacy.h"

@implementation EverydayPrivacy
+(void)requestTrackingAuthorization:(AuthorizationRequestCallback)callbackPointer{
    if (@available(iOS 14, *)) {
        [ATTrackingManager requestTrackingAuthorizationWithCompletionHandler:^(ATTrackingManagerAuthorizationStatus status) {
            callbackPointer(status);
        }];
    }else{
        callbackPointer(3);
    }
}

+(NSString*)getLocale{
    NSString *countryCode = [[NSLocale currentLocale] objectForKey: NSLocaleCountryCode];
    return countryCode;
}
@end

char* convertNSStringToCString(const NSString* nsString){
    if (nsString == NULL)
        return NULL;

    const char* nsStringUtf8 = [nsString UTF8String];

    char* cString = (char*)malloc(strlen(nsStringUtf8) + 1);
    strcpy(cString, nsStringUtf8);

    return cString;
}

extern "C" {
    void _RequestTrackingAuthorization(AuthorizationRequestCallback callback){
        [EverydayPrivacy requestTrackingAuthorization: callback];
    }

    char* _GetLocale(){
        return convertNSStringToCString([EverydayPrivacy getLocale]);
    }
}
