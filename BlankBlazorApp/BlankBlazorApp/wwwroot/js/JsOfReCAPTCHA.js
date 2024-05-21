function render_recaptcha(dotNetObj, selector, sitekey) {
    return grecaptcha.render(selector, {
        'sitekey': sitekey,
        'callback': (response) => { dotNetObj.invokeMethodAsync('CallbackOnSuccess', response); },
        'expired-callback': () => { dotNetObj.invokeMethodAsync('CallbackOnExpired'); },
        'error-callback': () => { dotNetObj.invokeMethodAsync('CallbackOnError'); }
    });
};

function getResponse(widgetId) {
    return grecaptcha.getResponse(widgetId);
}

//function onloadCallbackReCaptcha() {
//    alert('onloadCallbackReCaptcha');
//}error-callback