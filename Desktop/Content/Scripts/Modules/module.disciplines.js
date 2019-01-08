'use strict';

const Disciplines = (function () {

    //Private -----------------------------------------
    let _self = {
        arr: []
    };

    const _get = function () {

        const vm = {
        }
        
        Global.post(`Discipline_Get`, vm)
            .done(function (data) {
                _self.arr = data;
            })
            .fail(function (data) {
                Validation.notification(2);
            });

    }

    //Public ------------------------------------------
    const get = function () {
        _get();
    }
    const getSelf = function () {
        return _self;
    }

    return {
        get: get,
        getSelf: getSelf
    }

})();