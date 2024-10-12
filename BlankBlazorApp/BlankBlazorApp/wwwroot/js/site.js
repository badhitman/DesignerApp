////$.validator.addMethod('unlike',
////    function (value, element, params) {
////        var propertyValue = $(params[0]).val();
////        var dependentPropertyValue = $(params[1]).val();
////        return propertyValue !== dependentPropertyValue;
////    });

////$.validator.unobtrusive.adapters.add('unlike',
////    ['property'],
////    function (options) {
////        var element = $(options.form).find('#' + options.params['property'])[0];
////        options.rules['unlike'] = [element, options.element];
////        options.messages['unlike'] = options.message;
////    });

function SetDotNetHelper(dotNetHelper) {
    window.dotNetHelper = dotNetHelper;
}

function js_upload_handler(blobInfo, success, failure, progress) {
    console.log(JSON.stringify(blobInfo));
    window.dotNetHelper.invokeMethodAsync('UploadHandler', blobInfo.base64(), blobInfo.filename())
        .then((data) => {
            success(data);
        });
}

window.BoundingClientRect = (() => {
    return {
        Height(id) {
            var _d = $(`#${id}`);
            if (_d.length == 0)
                return 0;

            let height = _d.height();
            console.warn(JSON.stringify(height));

            if (isNaN(height)) {
                return 0;
            }

            return height;
        },
        Width(id) {
            var _d = $(`#${id}`);
            if (_d.length == 0)
                return 0;

            let width = _d.width();

            console.warn(JSON.stringify(width));

            if (isNaN(width)) {
                return 0;
            }

            return width;
        },
        X(id) {
            var _d = $(`#${id}`);
            if (_d.length == 0)
                return 0;

            var _p = _d.position();
            let left = _p.left;
            console.warn(JSON.stringify(left));

            if (isNaN(left)) {
                return 0;
            }

            return left;
        },
        Y(id) {
            var _d = $(`#${id}`);
            if (_d.length == 0)
                return 0;

            var _p = _d.position();
            let top = _p.top;
            console.warn(JSON.stringify(top));

            if (isNaN(top)) {
                return 0;
            }

            return top;
        }
    };
})();
function autoGrow(el) {
    if (el.style == undefined)
        return;

    el.style.height = '5px';
    el.style.height = el.scrollHeight + 'px';
}

window.autoGrowManage = (() => {
    return {
        registerGrow(dom_id, dotNetReference) {
            autoGrow(this);
            if (this.scrollHeight !== undefined)
                dotNetReference.invokeMethodAsync('EditorDataChanged', this.scrollHeight);
        }
    };
})();

window.methods = {
    CreateCookie: function (name, value, seconds, path) {
        console.warn(`call -> methods.CreateCookie(name:${name}, value:${value}, seconds:${seconds}, path:${path})`);
        var expires;
        if (seconds) {
            var date = new Date();
            date.setTime(date.getTime() + (seconds * 1000));
            expires = "; expires=" + date.toGMTString();
        }
        else {
            expires = "";
        }
        document.cookie = name + "=" + value + expires + `; path=${path}`;
    },
    UpdateCookie: function (name, seconds, path) {
        let value = window.methods.ReadCookie(name);
        console.warn(`call -> methods.UpdateCookie(name:${name}, seconds:${seconds}, path:${path}); set:${value}`);
        window.methods.CreateCookie(name, value, seconds, path);
    },
    ReadCookie: function (cname) {
        console.warn(`call -> methods.ReadCookie(cname:${cname})`);
        var name = cname + "=";
        var decodedCookie = decodeURIComponent(document.cookie);
        var ca = decodedCookie.split(';');
        for (var i = 0; i < ca.length; i++) {
            var c = ca[i];
            while (c.charAt(0) == ' ') {
                c = c.substring(1);
            }
            if (c.indexOf(name) == 0) {
                return c.substring(name.length, c.length);
            }
        }
        return "";
    },
    DeleteCookie: function (name, path, domain) {
        console.warn(`call -> methods.DeleteCookie(name:${name}, path:${path}, domain:${domain})`);
        if (window.methods.ReadCookie(name)) {
            document.cookie = name + "=" +
                ((path) ? ";path=" + path : "") +
                ((domain) ? ";domain=" + domain : "") +
                ";expires=Thu, 01 Jan 1970 00:00:01 GMT";
        }
    }
}

window.clipboardCopy = {
    copyText: function (text) {
        parent.navigator.clipboard.writeText(text).then(function () {
            //alert("Copied to clipboard!");
        })
            .catch(function (error) {
                alert(error);
            });
    }
}

window.bootstrapTheme = {
    IsDark: function () {
        let attrName = 'data-bs-theme';
        var is_dark = document.documentElement.getAttribute(attrName);
        if (is_dark == 'light')
            document.documentElement.setAttribute(attrName, 'dark');
        else
            document.documentElement.setAttribute(attrName, 'light');
    }
}