'use strict';

const Template = (function () {

    //Private ------------------------------------------------
    let _self = {
        timeout: undefined,
        records: 100,
        page: 1,
        sort: `name asc`,
        isShowMore: false,
        name: `Template`,
        arr: [],
        vm: {},
        constructor: function (materialId, name, isDeleted) {
            this.materialId = materialId;
            this.name = name;
            this.isDeleted = isDeleted;
        }
    };

    //Public ------------------------------------------------
    const getSelf = function () {
        return _self;
    };

    const _init = (function () {

    })();

    return {
        getSelf: getSelf
    }

})();