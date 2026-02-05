# Page snapshot

```yaml
- generic [ref=e2]:
  - generic [ref=e3]:
    - img [ref=e5]
    - heading "مستشفى آسيا" [level=3] [ref=e8]
    - paragraph [ref=e9]: Asia Hospital
  - generic [ref=e10]:
    - heading "Login" [level=4] [ref=e11]
    - generic [ref=e12]:
      - generic [ref=e13]:
        - generic [ref=e14]: Username or email address
        - textbox "Username or email address" [ref=e15]
      - generic [ref=e16]:
        - generic [ref=e17]: Password
        - textbox "Password" [ref=e18]
      - generic [ref=e19]:
        - generic [ref=e20]:
          - checkbox "Remember me" [ref=e21]
          - generic [ref=e22]: Remember me
        - link "Forgot password?" [ref=e23] [cursor=pointer]:
          - /url: /Account/ForgotPassword
      - button "Login" [ref=e24] [cursor=pointer]
  - generic [ref=e28]:
    - generic [ref=e29]: 
    - link "العربية" [ref=e30] [cursor=pointer]:
      - /url: /Abp/Languages/Switch?culture=ar&uiCulture=ar&returnUrl=%2Fconnect%2Fauthorize%3Fresponse_type%3Dcode%26client_id%3DHIS_App%26state%3DNTJmRFFaSGhsMjNFY2FqOWF6TnlCQkhHQWZ3Z3Z5bGlFNlRXamtMTV9ZNWxn%3B%25252Fpatients%26redirect_uri%3Dhttp%253A%252F%252Flocalhost%253A4200%26scope%3Dopenid%2520offline_access%2520HIS%26code_challenge%3DcVQCw_AAWFcllk9WvgL3pGqDIgcG_O7eb7AJ8DkttS4%26code_challenge_method%3DS256%26nonce%3DNTJmRFFaSGhsMjNFY2FqOWF6TnlCQkhHQWZ3Z3Z5bGlFNlRXamtMTV9ZNWxn%26culture%3Den%26ui-culture%3Den%26returnUrl%3D%252Fpatients
    - generic [ref=e31]: "|"
    - link "English" [ref=e32] [cursor=pointer]:
      - /url: /Abp/Languages/Switch?culture=en&uiCulture=en&returnUrl=%2Fconnect%2Fauthorize%3Fresponse_type%3Dcode%26client_id%3DHIS_App%26state%3DNTJmRFFaSGhsMjNFY2FqOWF6TnlCQkhHQWZ3Z3Z5bGlFNlRXamtMTV9ZNWxn%3B%25252Fpatients%26redirect_uri%3Dhttp%253A%252F%252Flocalhost%253A4200%26scope%3Dopenid%2520offline_access%2520HIS%26code_challenge%3DcVQCw_AAWFcllk9WvgL3pGqDIgcG_O7eb7AJ8DkttS4%26code_challenge_method%3DS256%26nonce%3DNTJmRFFaSGhsMjNFY2FqOWF6TnlCQkhHQWZ3Z3Z5bGlFNlRXamtMTV9ZNWxn%26culture%3Den%26ui-culture%3Den%26returnUrl%3D%252Fpatients
```