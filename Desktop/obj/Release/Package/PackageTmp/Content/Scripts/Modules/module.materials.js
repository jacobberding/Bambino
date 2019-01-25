'use strict';

const Materials = (function () {

    //Private ------------------------------------------------
    let _self = {
        timeout: undefined,
        records: 100,
        page: 1,
        arr: [],
        sort: `name asc`,
        isShowMore: false,
        vm: {},
        name: `Material`,
        constructor: function (materialId, disciplineId, name, description, website, priceMin, priceMax,
            materialPriceOptionKey, manufacturer, modelNumber, notes, isDeleted) {
            this.materialId = materialId;
            this.disciplineId = disciplineId;
            this.name = name;
            this.description = description;
            this.website = website;
            this.priceMin = priceMin;
            this.priceMax = priceMax;
            this.materialPriceOptionKey = materialPriceOptionKey;
            this.manufacturer = manufacturer;
            this.modelNumber = modelNumber;
            this.notes = notes;
            this.isDeleted = isDeleted;
        }
    }

    const _addEdit = function () {

        _self.vm.disciplineId = $(`#dboDisciplineId${_self.name}`).val();
        _self.vm.name = $(`#txtName${_self.name}`).val();
        _self.vm.description = $(`#txtDescription${_self.name}`).val();
        _self.vm.website = $(`#txtWebsite${_self.name}`).val();
        _self.vm.priceMin = $(`#numPriceMin${_self.name}`).nval();
        _self.vm.priceMax = $(`#numPriceMax${_self.name}`).nval();
        _self.vm.materialPriceOptionKey = $(`#dboMaterialPriceOptionKey${_self.name}`).val();
        _self.vm.manufacturer = $(`#txtManufacturer${_self.name}`).val();
        _self.vm.modelNumber = $(`#txtModelNumber${_self.name}`).val();
        _self.vm.notes = $(`#txtNotes${_self.name}`).val();
        _self.vm.isDeleted = false;

        _addEditDelete();

    }
    const _addEditDelete = function () {

        try {

            Validation.getIsValidForm($('m-module'));
            
            Global.post(`Material_AddEditDelete`, _self.vm)
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

    }

    const _delete = function () {

        _self.vm.isDeleted = true;

        _addEditDelete();

    }

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
        Global.post(`Material_GetByPage`, vm)
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

    }
    const _getById = function (id) {

        const vm = {
            id: id
        }

        Global.post(`Material_GetById`, vm)
            .done(function (data) {
                console.log(data);

                _self.vm = data;

                $(`m-module m-body`).html(getHtmlBodyDetail());

            }).fail(function (data) {
                Validation.notification(2);
            });

    }

    const _getEmptyVM = function () {
        return new _self.constructor(Global.guidEmpty,Global.guidEmpty,``,``,``,0,0,0,``,``,``,false);
    }

    const _search = function () {

        clearTimeout(_self.timeout);
        _self.timeout = setTimeout(function () {
            _getByPage(1);
        }, Global.keyUpTimeout);

    }
    const _sort = function ($this) {

        var dir = ($this.hasClass(`sortasc`)) ? `desc` : `asc`;

        _self.sort = `${$this.attr(`data-sort`)} ${dir}`;
        $(`.sort h2`).removeClass(`sortasc`).removeClass(`sortdesc`);
        $this.addClass(`sort${dir}`);
        _getByPage(1);
        
    }

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
    }
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
    }

    const getHtmlBody = function () {
        _getByPage(1);
        MaterialPriceOptions.get();
        Disciplines.get();
        return `

            <m-flex data-type="col" id="flx${_self.name}s">

                <m-flex data-type="row" class="c n pL pR">

                    <h1 class="w">${_self.name}</h1>

                    <m-flex data-type="row" class="w n">

                        <m-input class="n pR">
                            <input type="text" id="txtSearch${_self.name}" placeholder="Search" value="" required />
                        </m-input>

                        <m-flex data-type="row" class="n c sm sQ primary btnOpenModule" data-function="Materials.getHtmlModuleAdd">
                            <i class="icon-plus"><svg><use xlink:href="/Content/Images/Bambino.min.svg#icon-plus"></use></svg></i>
                        </m-flex>

                        <!--<m-flex data-type="row" class="n pL pR">

                            <m-button data-type="primary" class="" id="btnUpload${_self.name}">
                                Upload
                            </m-button>
                            <input type="file" class="none" id="upl${_self.name}" />

                        </m-flex>-->

                    </m-flex>

                </m-flex>
            
                ${Global.getHtmlLoading()}

            </m-flex>

            `;
    }
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
                    <h2 class="${Global.getSort(_self.sort, `discipline.value`)}" data-sort="discipline.value">
                        Discipline
                    </h2>
                    <h2 class="${Global.getSort(_self.sort, `manufacturer`)}" data-sort="manufacturer">
                        Manufacturer
                    </h2>
                    <h2 class="${Global.getSort(_self.sort, `modelNumber`)}" data-sort="modelNumber">
                        Model Number
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
    }
    const getHtmlBodyDetail = function () {
        return `

            <m-flex data-type="row" class="n pL pR">

                <m-flex data-type="col" class="n">
                    <h1>${_self.name}</h1>
                    <label>Settings</label>
                </m-flex>

                <m-flex data-type="row" class="n c sm sQ secondary btnOpenModule" data-function="Module.getHtmlConfirmation" data-args="material,btnDelete${_self.name}">
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
    }
    const getHtmlBodyForm = function () {

        let html = ``;

        for (let materialTag of _self.vm.materialTags)
            html += MaterialTag.getHtmlTag(materialTag);

        return `

            <m-flex data-type="col" class="form">

                <m-flex data-type="row" class="n">
                    <m-input class="mR">
                        <label for="txtName${_self.name}">Name</label>
                        <input type="text" id="txtName${_self.name}" placeholder="Name" value="${_self.vm.name}" />
                    </m-input>

                    <m-input>
                        <label for="dboDisciplineId${_self.name}">Discipline</label>
                        <select id="dboDisciplineId${_self.name}">
                            ${Global.getHtmlOptions(Disciplines.getSelf().arr, [_self.vm.disciplineId])}
                        </select>
                    </m-input>
                </m-flex>

                <m-flex data-type="row" class="n">
                    <m-input class="mR">
                        <label for="numPriceMin${_self.name}">Price Min</label>
                        <input type="number" id="numPriceMin${_self.name}" placeholder="Price Min" value="${_self.vm.priceMin}" />
                    </m-input>

                    <m-input class="mR">
                        <label for="numPriceMax${_self.name}">Price Max</label>
                        <input type="number" id="numPriceMax${_self.name}" placeholder="Price Max" value="${_self.vm.priceMax}" />
                    </m-input>

                    <m-input>
                        <label for="dboMaterialPriceOptionKey${_self.name}">Units</label>
                        <select id="dboMaterialPriceOptionKey${_self.name}">
                            ${Global.getHtmlOptions(MaterialPriceOptions.getSelf().arr, [_self.vm.materialPriceOptionKey])}
                        </select>
                    </m-input>
                </m-flex>

                <m-flex data-type="row" class="n">
                    <m-input class="mR">
                        <label for="txtManufacturer${_self.name}">Manufacturer</label>
                        <input type="text" id="txtManufacturer${_self.name}" placeholder="Manufacturer" value="${_self.vm.manufacturer}" />
                    </m-input>

                    <m-input class="mR">
                        <label for="txtModelNumber${_self.name}">Model Number</label>
                        <input type="text" id="txtModelNumber${_self.name}" placeholder="Model Number" value="${_self.vm.modelNumber}" />
                    </m-input>

                    <m-input>
                        <label for="txtWebsite${_self.name}">Website</label>
                        <input type="text" id="txtWebsite${_self.name}" placeholder="Website" value="${_self.vm.website}" />
                    </m-input>
                </m-flex>

                <m-flex data-type="row" class="n">
                    <m-input class="mR">
                        <label for="txtDescription${_self.name}">Description</label>
                        <input type="text" id="txtDescription${_self.name}" placeholder="Description" value="${_self.vm.description}" />
                    </m-input>

                    <m-input>
                        <label for="txtNotes${_self.name}">Notes</label>
                        <input type="text" id="txtNotes${_self.name}" placeholder="Notes" value="${_self.vm.notes}" />
                    </m-input>
                </m-flex>

                <m-flex data-type="row" class="n s">
                    <m-input class="mR">
                        <input type="text" id="txtSearchMaterialTag" placeholder="Tags" value="" />
                    </m-input>

                    <m-flex data-type="row" class="n c sm sQ secondary btnAddMaterialTag">
                        <i class="icon-plus"><svg><use xlink:href="/Content/Images/Bambino.min.svg#icon-plus"></use></svg></i>
                    </m-flex>
                </m-flex>

                <label for="txtSearchMaterialTag" class="mB">Material Tags</label>

                <m-flex data-type="row" class="n s" id="flxMaterialTags">
                    ${html}
                </m-flex>

            </m-flex>

            `;
    }

    const getHtmlCard = function (obj) {
        return `

            <m-card class="tableRow btnOpenModule mB" data-function="Materials.getHtmlModuleDetail" data-args="${obj.materialId}">
                <m-flex data-type="row" class="sC tE">
                    <h2 class="tE">
                        ${obj.name}
                    </h2>
                    <h2 class="tE">
                        ${obj.discipline.value}
                    </h2>
                    <h2 class="tE">
                        ${obj.manufacturer}
                    </h2>
                    <h2 class="tE">
                        ${obj.modelNumber}
                    </h2>
                </m-flex>
            </m-card>

            `;
    }

    const _init = (function () {
        $(document).on(`tap`, `#lst${_self.name}s .sort h2`, function () { _sort($(this)); });
        $(document).on(`tap`, `#btnAdd${_self.name}, #btnEdit${_self.name}`, function () { _addEdit(); });
        $(document).on(`tap`, `#btnDelete${_self.name}`, function () { _delete(); });
        $(document).on(`tap`, `#btnShowMore${_self.name}`, function () { _self.page++; _getByPage(_self.page); });
        $(document).on(`keyup`, `#txtSearch${_self.name}`, function () { _search(); });
        //$(document).on(`tap`, `#txtSearch${_self.name}`, function () { _search(); });
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

const MaterialTag = (function () {

    //Private -----------------------------------------
    let _self = {
        arr: [],
        vm: {},
        name: `MaterialTag`,
        timeout: undefined
    };

    const _add = function () {

        const vm = {
            materialId: Materials.getSelf().vm.materialId,
            name: $(`#txtSearch${_self.name}`).val()
        };

        Global.post(`Material_AddMaterialTag`, vm)
            .done(function (data) {

                $(`#flx${_self.name}s`).append(getHtmlTag(data));
                $(`#txtSearch${_self.name}`).val(``);

                Validation.notification(1);
            }).fail(function (data) {
                Validation.notification(1, `Error`, data.responseJSON.Message,`error`);
            });

    }

    const _delete = function ($this) {

        const vm = {
            materialId: Materials.getSelf().vm.materialId,
            materialTagId: $this.attr(`data-id`)
        };

        Global.post(`Material_DeleteMaterialTag`, vm)
            .done(function (data) {

                $(`m-card[data-id="${vm.materialTagId}"]`).remove();

                Validation.notification(1);
            }).fail(function (data) {
                Validation.notification(2);
            });

    }

    const _get = function () {

        Global.post(`${_self.name}_Get`, {})
            .done(function (data) {
                _self.arr = data;
            }).fail(function (data) {
                Validation.notification(2);
            });

    }

    const _search = function (e, $this) {

        if (e.which == 13) {
            
            $(`m-select[data-name="${_self.name}"]`).remove();
            _add();

            return;

        }

        let options = ``;

        $(`m-select[data-name="${_self.name}"]`).remove();

        if ($(`#txtSearch${_self.name}`).val() == ``)
            return;

        for (let obj of _self.arr.filter(function (obj) { return obj.name.toLowerCase().includes($(`#txtSearch${_self.name}`).val().toLowerCase()); }))
            options += `<m-option data-name="${_self.name}">${obj.name}</m-option>`;

        $this.parent().append(`<m-select data-name="${_self.name}">${options}</m-select>`);

    }
    const _select = function ($this) {

        $(`#txtSearch${_self.name}`).val($this.html()).focus();
        $(`m-select[data-name="${_self.name}"]`).remove();

    }

    //Public ------------------------------------------
    const get = function () {
        _get();
    };
    const getSelf = function () {
        return _self;
    };

    const getHtmlTag = function (obj) {
        return `

            <m-card class="tag" data-id="${obj.materialTagId}">
                <m-flex data-type="row" class="n">
                    <h1 class="tE">
                        ${obj.name}
                    </h1>
                    <m-flex data-type="row" class="n c xs sQ tertiary btnDelete${_self.name}" data-id="${obj.materialTagId}">
                        <i class="icon-delete"><svg><use xlink:href="/Content/Images/Bambino.min.svg#icon-delete"></use></svg></i>
                    </m-flex>
                </m-flex>
            </m-card>

            `;
    };

    const _init = (function () {
        $(document).on(`tap`, `m-option[data-name="${_self.name}"]`, function () { _select($(this)); });
        $(document).on(`keyup`, `#txtSearch${_self.name}`, function (e) { _search(e, $(this)); });
        $(document).on(`tap`, `.btnAdd${_self.name}`, function () { _add(); });
        $(document).on(`tap`, `.btnDelete${_self.name}`, function () { _delete($(this)); });
    })();

    return {
        get: get,
        getSelf: getSelf,
        getHtmlTag: getHtmlTag
    }

})();