'use strict';

const ACLayer = (function () {

    //Private ------------------------------------------------
    let _self = {
        timeout: undefined,
        records: 10,
        page: 1,
        arr: [],
        sort: `name asc`,
        isShowMore: false,
        vm: {},
        name: `ACLayer`,
        constructor: function (acLayerId, acLayerCategoryId, name, color, lineWeight, lineType, transparency,
            measurement, code, keywords, description, isPlottable, isDeleted) {
            this.acLayerId = acLayerId;
            this.acLayerCategoryId = acLayerCategoryId;
            this.name = name;
            this.color = color;
            this.lineWeight = lineWeight;
            this.lineType = lineType;
            this.transparency = transparency;
            this.measurement = measurement;
            this.code = code;
            this.keywords = keywords;
            this.description = description;
            this.isPlottable = isPlottable;
            this.isDeleted = isDeleted;
        }
    };

    const _addEdit = function () {

        _self.vm.acLayerCategoryId = $(`#dboACLayerCategoryId${_self.name}`).val();
        _self.vm.color = $(`#txtColor${_self.name}`).val();
        _self.vm.lineWeight = $(`#dboLineWeight${_self.name}`).val();
        _self.vm.lineType = $(`#dboLineType${_self.name}`).val();
        _self.vm.transparency = $(`#txtTransparency${_self.name}`).val();
        _self.vm.measurement = $(`#dboMeasurement${_self.name}`).val();
        _self.vm.code = $(`#txtCode${_self.name}`).val();
        _self.vm.keywords = $(`#txtKeywords${_self.name}`).val();
        _self.vm.description = $(`#txtDescription${_self.name}`).val();
        _self.vm.isPlottable = $(`#chkIsPlottable${_self.name}`).prop(`checked`);
        _self.vm.isDeleted = false;

        _addEditDelete();

    };
    const _addEditDelete = function () {

        try {

            Validation.getIsValidForm($('m-module'));

            Global.post(`${_self.name}_AddEditDelete`, _self.vm)
                .done(function (data) {
                    Validation.done();

                    _self.arr = [];

                    _getByPage(1);

                    Validation.notification(1);
                    Module.closeModuleAll();
                })
                .fail(function (data) {
                    Validation.fail(data);
                });

        } catch (ex) {
            Validation.fail(ex);
        }

    };

    const _delete = function () {

        _self.vm.isDeleted = true;

        _addEditDelete();

    };

    const _getByPage = function (page) {

        const vm = {
            page: page,
            records: _self.records,
            search: ($(`#txtSearch${_self.name}`).val()) ? $(`#txtSearch${_self.name}`).val() : ``,
            sort: _self.sort
        }

        if (page == 1) _self.arr = [];

        $(`m-body[data-label="Primary"] .flxLoading, m-body[data-label="Primary"] #lst${_self.name}s`).remove();
        $(`m-body[data-label="Primary"] #flx${_self.name}s`).append(Global.getHtmlLoading());

        _self.page = page;
        Global.post(`${_self.name}_GetByPage`, vm)
            .done(function (data) {

                _self.isShowMore = true;
                if (data.totalRecords < (vm.page * vm.records))
                    _self.isShowMore = false;

                for (let obj of data.arr)
                    _self.arr.push(obj);

                $(`m-body[data-label="Primary"] .flxLoading, m-body[data-label="Primary"] #lst${_self.name}s`).remove();
                $(`m-body[data-label="Primary"] #flx${_self.name}s`).append(getHtmlBodyList(_self.arr));

            })
            .fail(function (data) {
                Validation.notification(2);
            });

    };
    const _getById = function (id) {

        const vm = {
            id: id
        }

        Global.post(`${_self.name}_GetById`, vm)
            .done(function (data) {
                console.log(data);

                _self.vm = data;

                $(`m-module m-body`).html(getHtmlBodyDetail());

            }).fail(function (data) {
                Validation.notification(2);
            });

    };

    const _getEmptyVM = function () {
        return new _self.constructor(Global.guidEmpty, Global.guidEmpty, ``, ``, ``, ``, ``, ``, ``, ``, ``, false, false);
    };

    const _search = function () {

        clearTimeout(_self.timeout);
        _self.timeout = setTimeout(function () {
            _getByPage(1);
        }, Global.keyUpTimeout);

    };
    const _sort = function ($this) {

        var dir = ($this.hasClass(`sortasc`)) ? `desc` : `asc`;

        _self.sort = `${$this.attr(`data-sort`)} ${dir}`;
        $(`.sort h2`).removeClass(`sortasc`).removeClass(`sortdesc`);
        $this.addClass(`sort${dir}`);
        _getByPage(1);

    };

    //Public ------------------------------------------------
    const getSelf = function () {
        return _self;
    };

    const getHtmlModuleAdd = function () {
        _self.vm = _getEmptyVM();
        return `

            <m-header data-label="${_self.name} Add Header">
                <m-flex data-type="row" class="n">
                    <m-flex data-type="row" class="n c tab h active">
                        <span>Add</span>
                    </m-flex>
                </m-flex>
                <m-flex data-type="row" class="n c sQ h btnCloseModule">
                    <i class="icon-delete"><svg><use xlink:href="/Content/Images/Bambino.min.svg#icon-delete"></use></svg></i>
                </m-flex>
            </m-header>

            <m-body data-label="${_self.name} Add Body">

                <m-flex data-type="col" class="n pL pR">

                    <m-flex data-type="col" class="n">
                        <h1>${_self.name}</h1>
                        <label>Add</label>
                    </m-flex>
                
                </m-flex>

                ${getHtmlBodyForm()}

                <m-flex data-type="row" class="footer">
                    <m-button data-type="secondary" class="btnCloseModule">
                        Cancel
                    </m-button>

                    <m-button data-type="primary" id="btnAdd${_self.name}">
                        Add
                    </m-button>
                </m-flex>

            </m-body>
        
            `;
    };
    const getHtmlModuleDetail = function (id) {
        _getById(id);
        return `

            <m-header data-label="${_self.name} Detail Header">
                <m-flex data-type="row" class="n">
                    <m-flex data-type="row" class="n c tab h btnOpenBody" data-label="${_self.name} Detail Body" data-function="${_self.name}.getHtmlBodyDetail">
                        <span>Information</span>
                    </m-flex>
                </m-flex>
                <m-flex data-type="row" class="n c sQ h btnCloseModule">
                    <i class="icon-delete"><svg><use xlink:href="/Content/Images/Bambino.min.svg#icon-delete"></use></svg></i>
                </m-flex>
            </m-header>

            <m-body data-label="${_self.name} Detail Body">

                <m-flex data-type="col">

                    <h1 class="loading">Loading</h1>
                
                </m-flex>

            </m-body>
        
            `;
    };

    const getHtmlBody = function () {
        _getByPage(1);
        return `

            <m-flex data-type="col" id="flx${_self.name}s">

                <m-flex data-type="row" class="c n pL pR">

                    <h1 class="w">${_self.name}</h1>

                    <m-flex data-type="row" class="w n">

                        <m-input class="n pR">
                            <input type="text" id="txtSearch${_self.name}" placeholder="Search" value="" required />
                        </m-input>

                        <m-flex data-type="row" class="n c sm sQ mR primary btnOpenModule" data-function="${_self.name}.getHtmlModuleAdd">
                            <i class="icon-plus"><svg><use xlink:href="/Content/Images/Bambino.min.svg#icon-plus"></use></svg></i>
                        </m-flex>

                    </m-flex>

                </m-flex>
            
                ${Global.getHtmlLoading()}

            </m-flex>

            `;
    };
    const getHtmlBodyList = function (arr) {

        let html = ``;

        for (let obj of arr)
            html += getHtmlCard(obj);

        return `
            <m-flex data-type="col" class="s cards selectable" id="lst${_self.name}s">

                <m-flex data-type="row" class="tableRow n pL pR mB sC sort" style="width: 100%;">
                    <h2 class="${Global.getSort(_self.sort, `name`)}" data-sort="name">
                        Name
                    </h2>
                    <h2 class="${Global.getSort(_self.sort, `code`)}" data-sort="code">
                        Code
                    </h2>
                    <h2 class="${Global.getSort(_self.sort, `color`)}" data-sort="color">
                        Color
                    </h2>
                    <h2 class="${Global.getSort(_self.sort, `lineWeight`)}" data-sort="lineWeight">
                        Line Weight
                    </h2>
                </m-flex>

                ${html}

                ${(_self.isShowMore) ? `
                <m-card class="load" id="btnShowMore${_self.name}">
                    <m-flex data-type="row" class="c">
                        <h2>
                            Show More
                        </h2>
                    </m-flex>
                </m-card>` : ``}

            </m-flex>
            `;
    };
    const getHtmlBodyDetail = function () {
        return `

            <m-flex data-type="row" class="n pL pR">

                <m-flex data-type="col" class="n">
                    <h1>Layer</h1>
                    <label>Settings</label>
                </m-flex>

                <m-flex data-type="row" class="n c sm sQ secondary btnOpenModule" data-function="Module.getHtmlConfirmation" data-args="layer,btnDelete${_self.name}">
                    <i class="icon-trash-can"><svg><use xlink:href="/Content/Images/Bambino.min.svg#icon-trash-can"></use></svg></i>
                </m-flex>
            
            </m-flex>

            ${getHtmlBodyForm()}

            <m-flex data-type="row" class="footer">
                <m-button data-type="secondary" class="btnCloseModule">
                    Cancel
                </m-button>

                <m-button data-type="primary" id="btnEdit${_self.name}">
                    Save
                </m-button>
            </m-flex>

            `;
    };
    const getHtmlBodyForm = function () {
        return `

            <m-flex data-type="col" class="form">

                <m-flex data-type="row" class="n">
                    <m-input class="mR">
                        <label for="dboACLayerCategoryId${_self.name}">Category</label>
                        <select id="dboACLayerCategoryId${_self.name}">
                            ${Global.getHtmlOptions(ACLayerCategory.getSelf().arr, [_self.vm.acLayerCategoryId.toUpperCase()])}
                        </select>
                    </m-input>

                    <m-input>
                        <label for="txtColor${_self.name}">Color</label>
                        <input type="text" id="txtColor${_self.name}" placeholder="Color" value="${_self.vm.color}" />
                    </m-input>
                </m-flex>

                <m-flex data-type="row" class="n">
                    <m-input class="mR">
                        <label for="dboLineWeight${_self.name}">Line Weight</label>
                        <select id="dboLineWeight${_self.name}">
                            ${Global.getHtmlOptions(listLineWeights, [_self.vm.lineWeight])}
                        </select>
                    </m-input>

                    <m-input>
                        <label for="dboLineType${_self.name}">Line Type</label>
                        <select id="dboLineType${_self.name}">
                            ${Global.getHtmlOptions(listLineTypes, [_self.vm.lineType])}
                        </select>
                    </m-input>
                </m-flex>

                <m-flex data-type="row" class="n">
                    <m-input class="mR">
                        <label for="txtTransparency${_self.name}">Transparency</label>
                        <input type="text" id="txtTransparency${_self.name}" placeholder="Transparency" value="${_self.vm.transparency}" />
                    </m-input>

                    <m-input>
                        <label for="dboMeasurement${_self.name}">Measurement</label>
                        <select id="dboMeasurement${_self.name}">
                            ${Global.getHtmlOptions(listScales, [_self.vm.measurement])}
                        </select>
                    </m-input>
                </m-flex>

                <m-flex data-type="row" class="n">
                    <m-input class="mR">
                        <label for="txtCode${_self.name}">Code</label>
                        <input type="text" id="txtCode${_self.name}" placeholder="Code" value="${_self.vm.code}" />
                    </m-input>

                    <m-input>
                        <label for="txtKeywords${_self.name}">Keywords</label>
                        <input type="text" id="txtKeywords${_self.name}" placeholder="Keywords" value="${_self.vm.keywords}" />
                    </m-input>
                </m-flex>

                <m-flex data-type="row" class="n">
                    <m-input class="mR">
                        <label for="txtDescription${_self.name}">Description</label>
                        <input type="text" id="txtDescription${_self.name}" placeholder="Description" value="${_self.vm.description}" />
                    </m-input>
                </m-flex>

                <m-flex data-type="row" class="n">
                    <m-input class="mR">
                        <m-flex data-type="row" class="n">
                            <input type="checkbox" class="mR" id="chkIsPlottable${_self.name}" ${(_self.vm.isPlottable) ? `checked` : ``} />
                            <label for="chkIsPlottable${_self.name}">Is Plottable</label>
                        </m-flex>
                    </m-input>
                </m-flex>

            </m-flex>

            `;
    };

    const getHtmlCard = function (obj) {
        return `

            <m-card class="tableRow btnOpenModule mB" data-function="${_self.name}.getHtmlModuleDetail" data-args="${obj.acLayerId}">
                <m-flex data-type="row" class="sC tE">
                    <h2 class="tE">
                        ${obj.name}
                    </h2>
                    <h2 class="tE">
                        ${obj.code}
                    </h2>
                    <h2 class="tE">
                        ${obj.color}
                    </h2>
                    <h2 class="tE">
                        ${obj.lineWeight}
                    </h2>
                </m-flex>
            </m-card>

            `;
    };

    const _init = (function () {
        $(document).on(`tap`, `#lst${_self.name}s .sort h2`, function () { _sort($(this)); });
        $(document).on(`tap`, `#btnAdd${_self.name}, #btnEdit${_self.name}`, function () { _addEdit(); });
        $(document).on(`tap`, `#btnDelete${_self.name}`, function () { _delete(); });
        $(document).on(`tap`, `#btnShowMore${_self.name}`, function () { _self.page++; _getByPage(_self.page); });
        $(document).on(`keyup`, `#txtSearch${_self.name}`, function () { _search(); });
        $(document).on(`tap`, `#txtSearch${_self.name}`, function () { _search(); });
    })();

    return {
        getSelf: getSelf,
        getHtmlModuleAdd: getHtmlModuleAdd,
        getHtmlModuleDetail: getHtmlModuleDetail,
        getHtmlBody: getHtmlBody,
        getHtmlBodyList: getHtmlBodyList,
        getHtmlBodyDetail: getHtmlBodyDetail,
        getHtmlBodyForm: getHtmlBodyForm,
        getHtmlCard: getHtmlCard
    }

})();
const ACLayerCategory = (function () {

    //Private -----------------------------------------
    let _self = {
        arr: [],
        name: `ACLayerCategory`
    };

    const _get = function () {

        const vm = {
        }

        Global.post(`${_self.name}_Get`, vm)
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