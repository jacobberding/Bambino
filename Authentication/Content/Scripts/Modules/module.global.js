'use strict';

//Jquery get number from input val
$.fn.nval = function() {
   return Number(this.val())
};
Number.prototype.formatCurrency = function (n, x) {
    var re = '\\d(?=(\\d{' + (x || 3) + '})+' + (n > 0 ? '\\.' : '$') + ')';
    return "$" + this.toFixed(Math.max(0, ~~n)).replace(new RegExp(re, 'g'), '$&,');
};

const Global = (function () {
    
    //Private ------------------------------------------------
    const _easing = [.54, -0.23, .45, 1.26];
    const _durationShort = 200;
    const _durationLong = 700;
    const _allowedFileExtensions = ['jpg', 'jpeg', 'png'];
    let _timeout;
    
    const _getHostName = function (url) {
        var match = url.match(/:\/\/(www[0-9]?\.)?(.[^/:]+)/i);
        if (match != null && match.length > 2 && typeof match[2] === 'string' && match[2].length > 0) {
        return match[2];
        }
        else {
            return null;
        }
    }
    const _getDomain = function (url) {
        var hostName = _getHostName(url);
        var domain = hostName;
    
        if (hostName != null) {
            var parts = hostName.split('.').reverse();
        
            if (parts != null && parts.length > 1) {
                domain = parts[1] + '.' + parts[0];
                
                if (hostName.toLowerCase().indexOf('.co.uk') != -1 && parts.length > 2) {
                  domain = parts[2] + '.' + domain;
                }
            }
        }
    
        return domain;
    }
    
    const _post = function (m, vm) {
        //console.log(`m`, m);
        //console.log(`vm`, vm);
        Global.ajaxInProgress = true;
        return $.ajax({
            type: "POST",
            url: '/Methods/Get',
            data: {
                vm: JSON.stringify(vm),
                m: m
            },
            dataType: "json",
            success: function (data) {
                console.log(`Success`, { m: m, vm: vm, data: data });
                Global.ajaxInProgress = false;
            },
            error: function (data) {
                console.log(`Error`, { m: m, vm: vm, data: data });
                Global.ajaxInProgress = false;
                //Global.addError(data.responseJSON.Message, window.location.protocol + "//" + window.location.host, ``, m, JSON.stringify(vm));
            }
        });
    }
    const _upload = function (formData, success, url) {
        
        $.ajax({
            xhr: function () {
                var xhr = new window.XMLHttpRequest();
                xhr.upload.addEventListener("progress", function (evt) {
                    if (evt.lengthComputable) {
                        $(`m-bar.progress span`).css(`width`, `${parseInt((evt.loaded / evt.total) * 100).toFixed(0)}%`);
                    }
                }, false);
        
                xhr.addEventListener("progress", function (evt) {
                    if (evt.lengthComputable) {
                        $(`m-bar.progress span`).css(`width`, `${parseInt((evt.loaded / evt.total) * 100).toFixed(0)}%`);
                    }
                }, false);
        
                return xhr;
            },
            type: "POST",
            url: url,
            data: formData,
            dataType: 'json',
            cache: false,
            contentType: false,
            processData: false,
            success: function (data) { success(data); },
            error: function (data) { 
                console.log(data); 
            }
        });

    }
    
    const _signOut = function () {
        
        Global.jack.mT = Global.guidEmpty;

        $.ajax({
            type: "POST",
            url: '/Methods/SetJackSparrow',
            data: { value: JSON.stringify(Global.jack) },
            dataType: "json",
            success: function (data) {
                Global.deleteCookie('_e');
                Global.addCookie('_e', data, 365);
                window.location = window.location.protocol + "//" + window.location.host;
            }
        });

    }

    //Public ------------------------------------------------
    let jack;
    let ajaxInProgress = false;
    //let timeZone = moment.tz.guess();
    const guidEmpty = `00000000-0000-0000-0000-000000000000`;
    const keyUpTimeout = 500;
    const velocitySettings = {
        easing: _easing,
        durationShort: _durationShort,
        durationLong: _durationLong,
        options: { duration: _durationShort, easing: _easing },
        optionsFlex: { display: 'flex', duration: _durationShort, easing: _easing }
    }
    const isStudio = function () {
        return ($(`m-body[data-label="Studio"]`).length > 0) ? true : false;
    }
    const isMobile = function () {
        return (/Android|webOS|iPhone|iPod|BlackBerry|IEMobile|Opera Mini/i.test(navigator.userAgent) || window.matchMedia("(max-width: 768px)").matches) ? true : false;
    }
    
    const addCookie = function (name, value, days) {
        var expires;

        if (days) {
            var date = new Date();
            date.setTime(date.getTime() + (days * 24 * 60 * 60 * 1000));
            expires = "; expires=" + date.toGMTString();
        } else {
            expires = "";
        }
        document.cookie = name + "=" + value + expires + "; path=/;domain=." + _getDomain(window.location.href);
    }
    
    const editJackSparrow = function () {
        $.ajax({
            type: "POST",
            url: '/Methods/SetJackSparrow',
            data: { value: JSON.stringify(Global.jack) },
            dataType: "json",
            success: function (data) {
                Global.deleteCookie('_c');
                Global.addCookie('_c', data, 365);
            }
        });
    }

    const deleteCookie = function (name) {
        Global.addCookie(name, "", -1);
    }

    const getNewGuid = function () {
        //function s4() {
        //    return Math.floor((1 + Math.random()) * 0x10000).toString(16).substring(1);
        //}
        return Math.floor((1 + Math.random()) * 0x10000).toString(16).substring(1) + Math.floor((1 + Math.random()) * 0x10000).toString(16).substring(1) + '-' + Math.floor((1 + Math.random()) * 0x10000).toString(16).substring(1) + '-' + Math.floor((1 + Math.random()) * 0x10000).toString(16).substring(1) + '-' + Math.floor((1 + Math.random()) * 0x10000).toString(16).substring(1) + '-' + Math.floor((1 + Math.random()) * 0x10000).toString(16).substring(1) + Math.floor((1 + Math.random()) * 0x10000).toString(16).substring(1) + Math.floor((1 + Math.random()) * 0x10000).toString(16).substring(1);
    }
    const getFunctionByName = function (functionName, args) {
        
        const namespaces = functionName.split(".");
        const func = namespaces.pop();
        let ns = namespaces.join('.');

        if(ns == '')
            ns = 'window';

        ns = eval(ns);
        
        return (typeof(args) === 'string') ? ns[func].apply( this, args.split(`,`) ) : ns[func](args);

    }
    const getIsValidFile = function (obj) {
        return ($.inArray(obj.name.split('.').pop().toLowerCase(), _allowedFileExtensions) > -1) ? true : false; // || /^[^<>%$]*$/.test(obj.name)
    }
    const getThumbnailPath = function (path) {
        
        const filename = path.split('/').pop();
        const pathBase = path.split('/').slice(0, -1).join('/');
    
        return pathBase + `/thumbnails/` + filename;

    }
    const getSort = function (sort, name) {
        if (sort.includes(name))
            return `sort${sort.split(` `)[1]}`;
        else
            return ``;
    }
    
    const getHtmlOptions = function (arr, values = [], isFont = false) {

        let html = ``;
        
        //console.log(`arr`, arr);
        arr = arr.sort(function (a, b) {
            if (a.name < b.name) return -1;
            else if (a.name > b.name) return 1;
            return 0;
        });
        for (let obj of arr)
            html += `<option ${(isFont) ? `style="font-family: '${obj.value}';"` : ``} value='${obj.value}' ${(values.filter(function (i) { return i.value == obj.value || i == obj.value; }).length > 0) ? `selected` : ``}>${obj.name}</option>`;

        return html;

    }
    const getHtmlLoading = function () {
        return `
                <m-flex data-type="col" class="flxLoading c mT">
                    <h4 class="mB">We have a very slow server please be patient. This will be upgraded once we are ready.</h4>
                    <h1 class="loading">Loading</h1>
                </m-flex>
            `;
    }
    const getListNameByValue = function (arr, value) {
        return arr.filter(function (obj) { return obj.value == value; })[0].name;
    }
    const getListNameByValues = function (arr, values) {

        const a = arr.filter(function (obj) { return values.filter(function (i) { return i == obj.value; }).length > 0; });
        let html = ``;

        for (let b of a)
            html += `${b.name} / `;

        return $.trim(html).replace(/\/$/, " ");

    }
    const getListValueByName = function (arr, name) {
        return arr.filter(function (obj) { return obj.name == name; })[0].value;
    }

    const post = function (m, vm) {
        return _post(m, vm);
    }
    const upload = function (formData, success, url) {
        _upload(formData, success, url);
    }

    const _init = (function () {
        //$(document).on(`tap`, `#`, function () { window.location = `https://desktop.bambino.software`; });
        $(document).on(`tap`, `#btnSignOut`, function (e) { _signOut(); });

        if (!isMobile()) {
            $(document).on('mouseenter', '.tooltip', function (e) {

                _hiddenDiv.css(`font-size`, `10px`);
                _hiddenDiv.html($(this).attr(`data-label`));

                const left = (event.clientX > (window.outerWidth - 100)) ? parseInt(($(this).offset().left - 10) - $(_hiddenDiv).width()) : parseInt($(this).offset().left + 50);

                _hiddenDiv.html(``);
                
                $(`body`).append(`<m-tooltip style="left: ${left}px;top: ${parseInt($(this).offset().top + 15)}px;">${$(this).attr(`data-label`)}</m-tooltip>`);

            });
            $(document).on('mouseleave', '.tooltip', function () { $(`m-tooltip`).remove(); });
        }

    })();

    return {
        jack: jack,
        ajaxInProgress: ajaxInProgress,
        //timeZone: timeZone,
        guidEmpty: guidEmpty,
        keyUpTimeout: keyUpTimeout,
        velocitySettings: velocitySettings,
        isStudio: isStudio,
        isMobile: isMobile,
        addCookie: addCookie,
        editJackSparrow: editJackSparrow,
        deleteCookie: deleteCookie,
        getNewGuid: getNewGuid,
        getFunctionByName: getFunctionByName,
        getIsValidFile: getIsValidFile,
        getThumbnailPath: getThumbnailPath,
        getSort: getSort,
        getHtmlOptions: getHtmlOptions,
        getHtmlLoading: getHtmlLoading,
        getListNameByValue: getListNameByValue,
        getListNameByValues: getListNameByValues,
        getListValueByName: getListValueByName,
        post: post,
        upload: upload
    }

})();
