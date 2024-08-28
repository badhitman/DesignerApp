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

window.BoundingClientRect = (() => {
    return {
        Height(id) {
            var _d = $(`#${id}`);
            if (_d.length == 0)
                return 0;

            return _d.height();
        },
        Width(id) {
            var _d = $(`#${id}`);
            if (_d.length == 0)
                return 0;

            return _d.width();
        },
        X(id) {
            var _d = $(`#${id}`);
            if (_d.length == 0)
                return 0;

            var _p = _d.position();

            return _p.left;
        },
        Y(id) {
            var _d = $(`#${id}`);
            if (_d.length == 0)
                return 0;

            var _p = _d.position();

            return _p.top;
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

