'use strict';

const Project = (function () {

    //Private ------------------------------------------------
    let _self = {
        timeout: undefined,
        records: 5,
        page: 1,
        sort: `name asc`,
        isShowMore: false,
        vm: {},
        arr: [],
        name: `Project`,
        constructor: function (projectId, name, isDeleted) {
            this.projectId = projectId;
            this.name = name;
            this.isDeleted = isDeleted;
        }
    };

    const _addEdit = function () {
        
        _self.vm.name = $(`#txtName${_self.name}`).val();
        _self.vm.scale = $(`#dboScale${_self.name}`).val();
        _self.vm.isDeleted = false;

        _addEditDelete();

    }
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
        Global.post(`${_self.name}_GetByPage`, vm)
            .done(function (data) {

                _self.isShowMore = true;
                if (data.totalRecords <= (vm.page * vm.records))
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

        Global.post(`${_self.name}_GetById`, vm)
            .done(function (data) {
                console.log(data);

                _self.vm = data;

                $(`#flxBodyDetail${_self.name}`).html(getHtmlBodyDetail());
                $(`#flxHeaderDetail${_self.name}`).append(ProjectPhase.getHtmlHeader(data.projectPhases));

            }).fail(function (data) {
                Validation.notification(2);
            });

    }

    const _getEmptyVM = function () {
        return new _self.constructor(Global.guidEmpty, ``, false);
    }

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
    const getByPage = function (page) {
        _getByPage(page);
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

    const getHtmlBody = function () {
        _getByPage(1);
        return `

            <m-flex data-type="col" class="w" id="flx${_self.name}s">

                <m-flex data-type="row" class="c n pL pR">

                    <h1 class="w">${_self.name}s</h1>

                    <m-flex data-type="row" class="w n">

                        <m-input class="n pR">
                            <input type="text" id="txtSearch${_self.name}" placeholder="Search" value="" required />
                        </m-input>

                        ${Global.jack.mIA ? `
                        <m-flex data-type="row" class="n c sm sQ primary btnOpenModule" data-function="Project.getHtmlModuleAdd">
                            <i class="icon-plus"><svg><use xlink:href="/Content/Images/Bambino.min.svg#icon-plus"></use></svg></i>
                        </m-flex>` : ``}

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
                    <h2 class="${Global.getSort(_self.sort, `code`)}" data-sort="code">
                        Code
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
    const getHtmlBodyById = function (id) {
        _getById(id);

        const obj = _self.arr.filter(function (x) { return x.projectId == id; })[0];

        return `

            <m-flex data-type="col" class="n" id="flxHeaderDetail${_self.name}">

                <m-flex data-type="row" class="n pL pT pR">

                    <m-flex data-type="col" class="n btnOpenBody" data-label="Project" data-function="Project.getHtmlBodyDetail" data-args="">
                        <h1>${obj.name}</h1>
                        <label>Something</label>
                    </m-flex>

                    ${Global.jack.mIA ? `
                    <m-flex data-type="row" class="n c sm sQ secondary btnOpenModule" data-function="Module.getHtmlConfirmation" data-args="project,btnDelete${_self.name}">
                        <i class="icon-trash-can"><svg><use xlink:href="/Content/Images/Bambino.min.svg#icon-trash-can"></use></svg></i>
                    </m-flex>` : ``}
            
                </m-flex>

            </m-flex>

            <m-flex data-type="col" class="" id="flxBodyDetail${_self.name}">
                <h1 class="loading">Loading</h1>
            </m-flex>

            `;
    }
    const getHtmlBodyDetail = function () {
        return `

            <m-body data-label="Project" class="inh">

                <m-flex data-type="row" class="s n">

                    <m-flex data-type="col" class="w50 n mR">

                        <m-card class="crdInfo">
                            <m-flex data-type="col" class="">

                                <h6>Location</h6>

                                <m-flex data-type="row" class="n mB">
                                    <h5>${_self.vm.addressLine1} ${_self.vm.addressLine2}, ${_self.vm.city} ${_self.vm.state} ${_self.vm.zip} ${_self.vm.country}</h5>
                                </m-flex>

                                <h6>Scale</h6>

                                <m-flex data-type="row" class="n mB">
                                    <h5>${_self.vm.scale}</h5>
                                </m-flex>

                                <m-flex data-type="row" class="n s mB">

                                    <h6 class="mR">${_self.vm.numOfMembers}</h6>
                                    <m-flex data-type="row" class="n c xs sQ mR tertiary">
                                        <i class="icon-staff"><svg><use xlink:href="/Content/Images/Bambino.min.svg#icon-staff"></use></svg></i>
                                    </m-flex>

                                    <h6 class="mR">${_self.vm.numOfHours}</h6>
                                    <m-flex data-type="row" class="n c xs sQ mR tertiary">
                                        <i class="icon-clock"><svg><use xlink:href="/Content/Images/Bambino.min.svg#icon-clock"></use></svg></i>
                                    </m-flex>

                                </m-flex>

                            </m-flex>
                        </m-card>

                    </m-flex>

                    <m-flex data-type="row" class="n w wR s">

                        ${Global.jack.mIA ? `
                        <m-button data-type="secondary" class="sQ btnOpenBody mR" data-label="Project" data-function="TimeTrackerProject.getHtmlBody" data-args="${_self.vm.projectId}">
                            Time Sheets
                        </m-button>` : ``}

                        <m-button data-type="secondary" class="sQ btnOpenBody mR" data-label="Project" data-function="ProjectMember.getHtmlBody" data-args="">
                            Team
                        </m-button>

                        <m-button data-type="secondary" class="sQ btnOpenBody mR" data-label="Project" data-function="ProjectZone.getHtmlBody" data-args="">
                            Zones
                        </m-button>

                        <m-button data-type="secondary" class="sQ btnOpenBody mR" data-label="Project" data-function="Project.getHtmlBodySettings" data-args="">
                            Settings
                        </m-button>

                    </m-flex>

                </m-flex>

            </m-body>

            `;
    }
    const getHtmlBodyForm = function () {
        return `

            <m-flex data-type="col" class="form">

                <m-flex data-type="row" class="n">

                    <m-input class="mR">
                        <label for="txtName${_self.name}">Name</label>
                        <input type="text" id="txtName${_self.name}" placeholder="Name" value="${_self.vm.name}" />
                    </m-input>

                </m-flex>

            </m-flex>

            `;
    }
    const getHtmlBodySettings = function () {
        return `

            <m-flex data-type="row" class="n s w">

                <m-flex data-type="col" class="w">

                    <h1>Settings</h1>

                    <m-input class="mR">
                        <label for="txtName${_self.name}">Name</label>
                        <input type="text" id="txtName${_self.name}" placeholder="Name" value="${_self.vm.name}" />
                    </m-input>

                    <m-input>
                        <label for="dboScale${_self.name}">Scale</label>
                        <select id="dboScale${_self.name}">
                            ${Global.getHtmlOptions(listScales, [_self.vm.scale])}
                        </select>
                    </m-input>

                    <m-flex data-type="row" class="footer">
                        <m-button data-type="primary" id="btnEdit${_self.name}">
                            Save
                        </m-button>
                    </m-flex>

                </m-flex>

                ${ProjectPhase.getHtmlBody()}

            </m-flex>

            `;
    }

    const getHtmlCard = function (obj) {
        return `

            <m-card class="tableRow btnOpenBody mB" data-label="Primary" data-function="Project.getHtmlBodyById" data-args="${obj.projectId}">
                <m-flex data-type="row" class="sC tE">
                    <h2 class="tE">
                        ${obj.name}
                    </h2>
                    <h2 class="tE">
                        ${obj.code}
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
    })();

    return {
        getSelf: getSelf,
        getByPage: getByPage,
        getHtmlModuleAdd: getHtmlModuleAdd,
        getHtmlBody: getHtmlBody,
        getHtmlBodyList: getHtmlBodyList,
        getHtmlBodyById: getHtmlBodyById,
        getHtmlBodyDetail: getHtmlBodyDetail,
        getHtmlBodyForm: getHtmlBodyForm,
        getHtmlBodySettings: getHtmlBodySettings,
        getHtmlCard: getHtmlCard
    }

})();
const ProjectZone = (function () {

    //Private ------------------------------------------------
    let _self = {
        timeout: undefined,
        records: 100,
        page: 1,
        sort: `code asc`,
        isShowMore: false,
        name: `ProjectZone`,
        arr: [],
        vm: {},
        constructor: function (projectZoneId, projectId, name, description, code, isDeleted) {
            this.projectZoneId = projectZoneId;
            this.projectId = projectId;
            this.name = name;
            this.description = description;
            this.code = code;
            this.isDeleted = isDeleted;
        }
    };

    const _addEdit = function () {

        _self.vm.projectId = Project.getSelf().vm.projectId;
        _self.vm.name = $(`#txtName${_self.name}`).val();
        _self.vm.description = $(`#txtDescription${_self.name}`).val();
        _self.vm.isDeleted = false;

        _addEditDelete();

    }
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
            sort: _self.sort,
            id: Project.getSelf().vm.projectId,
            projectPhaseId: Project.getSelf().vm.projectPhaseId
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

    }

    const _getEmptyVM = function () {
        return new _self.constructor(Global.guidEmpty, Global.guidEmpty, ``, ``, 0, false);
    }

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
    const _archive = function ($this, isArchived) {

        const vm = {
            projectZoneId: $this.attr(`data-id`),
            isArchived: isArchived
        };

        Global.post(`${_self.name}_EditArchive`, vm)
            .done(function (data) {

                _self.arr = [];

                _getByPage(1);

                Validation.notification(1);
                Module.closeModuleAll();
            })
            .fail(function (data) {
                Validation.notification(2);
            });

    };

    //Public ------------------------------------------------
    const getSelf = function () {
        return _self;
    };
    const getCode = function (code) {
        return code > 9 ? code : `0${code}`;
    }

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
                        <h1>Project Zone</h1>
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

            <m-flex data-type="col" class="n w" id="flx${_self.name}s">

                <m-flex data-type="row" class="n">

                    <h1 class="w">Zones</h1>

                    <m-flex data-type="row" class="n c sm sQ tertiary btnOpenModule" data-function="${_self.name}.getHtmlModuleAdd" data-args="">
                        <i class="icon-plus"><svg><use xlink:href="/Content/Images/Bambino.min.svg#icon-plus"></use></svg></i>
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
            <m-flex data-type="col" class="n s cards selectable" id="lst${_self.name}s">

                <m-flex data-type="row" class="tableRow n pL mB sC sort" style="width: 100%;">
                    <h2 class="${Global.getSort(_self.sort, `code`)}" data-sort="code">
                        Code
                    </h2>
                    <h2 class="${Global.getSort(_self.sort, `name`)}" data-sort="name">
                        Name
                    </h2>
                    <m-flex data-type="row" class="n c sm sQ tertiary hidden">
                        <i class="icon-"><svg><use xlink:href="/Content/Images/Bambino.min.svg#icon-"></use></svg></i>
                    </m-flex>
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
                    <h1>${_self.name}</h1>
                    <label>Settings</label>
                </m-flex>

                <m-flex data-type="row" class="n c sm sQ secondary btnOpenModule" data-function="Module.getHtmlConfirmation" data-args="zone,btnDelete${_self.name}">
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
                        <label for="txtName${_self.name}">Name</label>
                        <input type="text" id="txtName${_self.name}" placeholder="Name" value="${_self.vm.name}" />
                    </m-input>
                </m-flex>

                <m-flex data-type="row" class="n">
                    <m-input class="n mR">
                        <label for="txtDescription${_self.name}">Description</label>
                        <textarea type="text" id="txtDescription${_self.name}" placeholder="Description">${_self.vm.description}</textarea>
                    </m-input>
                </m-flex>

            </m-flex>

            `;
    };

    const getHtmlCard = function (obj) {
        return `

            <m-card class="tableRow mB nT btnOpenModule ${obj.isArchived ? `archive` : ``}" data-function="${_self.name}.getHtmlModuleDetail" data-args="${obj.projectZoneId}">
                <m-flex data-type="row" class="n pL sC tE">
                    <h2 class="tE">
                        ${getCode(obj.code)}
                    </h2>
                    <h2 class="tE">
                        ${obj.name}
                    </h2>
                    ${Global.jack.mIM && !obj.isArchived ? `
                    <m-flex data-type="row" class="n c sm sQ tertiary btnOpenModule" data-function="Module.getHtmlConfirmation" data-args="Are you sure you want to archive this zone?,btnArchive${_self.name},${obj.projectZoneId}">
                        <i class="icon-downloads"><svg><use xlink:href="/Content/Images/Bambino.min.svg#icon-downloads"></use></svg></i>
                    </m-flex>` : ``}
                    ${Global.jack.mIM && obj.isArchived ? `
                    <m-flex data-type="row" class="n c sm sQ tertiary btnOpenModule" data-function="Module.getHtmlConfirmation" data-args="Are you sure you want to unarchive this zone?,btnUnarchive${_self.name},${obj.projectZoneId}">
                        <i class="icon-downloads"><svg><use xlink:href="/Content/Images/Bambino.min.svg#icon-downloads"></use></svg></i>
                    </m-flex>` : ``}
                </m-flex>
            </m-card>

            `;
    };

    const _init = (function () {
        $(document).on(`tap`, `#lst${_self.name}s .sort h2`, function () { _sort($(this)); });
        $(document).on(`tap`, `#btnAdd${_self.name}, #btnEdit${_self.name}`, function () { _addEdit(); });
        $(document).on(`tap`, `#btnArchive${_self.name}`, function () { _archive($(this), true); });
        $(document).on(`tap`, `#btnUnarchive${_self.name}`, function () { _archive($(this), false); });
        $(document).on(`tap`, `#btnDelete${_self.name}`, function () { _delete(); });
        $(document).on(`tap`, `#btnShowMore${_self.name}`, function () { _self.page++; _getByPage(_self.page); });
        $(document).on(`keyup`, `#txtSearch${_self.name}`, function () { _search(); });
    })();

    return {
        getSelf: getSelf,
        getCode: getCode,
        getHtmlModuleAdd: getHtmlModuleAdd,
        getHtmlModuleDetail: getHtmlModuleDetail,
        getHtmlBody: getHtmlBody,
        getHtmlBodyList: getHtmlBodyList,
        getHtmlBodyDetail: getHtmlBodyDetail,
        getHtmlBodyForm: getHtmlBodyForm,
        getHtmlCard: getHtmlCard
    };

})();
const ProjectPhase = (function () {

    //Private ------------------------------------------------
    let _self = {
        timeout: undefined,
        records: 100,
        page: 1,
        sort: `name asc`,
        isShowMore: false,
        name: `ProjectPhase`,
        arr: [],
        vm: {},
        constructor: function (projectPhaseId, projectId, name, description, sortOrder, dateStart, dateEnd, isDeleted) {
            this.projectPhaseId = projectPhaseId;
            this.projectId = projectId;
            this.name = name;
            this.description = description;
            this.sortOrder = sortOrder;
            this.dateStart = dateStart;
            this.dateEnd = dateEnd;
            this.isDeleted = isDeleted;
        }
    };

    const _addEdit = function () {

        _self.vm.projectId = Project.getSelf().vm.projectId;
        _self.vm.name = $(`#txtName${_self.name}`).val();
        _self.vm.description = $(`#txtDescription${_self.name}`).val();
        _self.vm.dateStart = moment($(`#dteDateStart${_self.name}`).val()).format(`YYYY-MM-DD Z`);
        _self.vm.dateEnd = moment($(`#dteDateEnd${_self.name}`).val()).format(`YYYY-MM-DD Z`);
        _self.vm.isDeleted = false;

        _addEditDelete();

    }
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
            sort: _self.sort,
            id: Project.getSelf().vm.projectId
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

    }

    const _getEmptyVM = function () {
        return new _self.constructor(Global.guidEmpty, Global.guidEmpty, ``, ``, 0, ``, ``, false);
    }

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
                        <h1>Project Phase</h1>
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

    const getHtmlHeader = function (arr) {

        let html = ``;

        for (let obj of arr) 
            html += `<span class="${Project.getSelf().vm.projectPhaseId == obj.projectPhaseId ? `active` : ``}">${obj.name.match(/\b(\w)/g).join('').toUpperCase()}</span>`;

        return `

            <m-flex data-type="row" class="s bC">
                ${html}
            </m-flex>

            `;
    };

    const getHtmlBody = function () {
        _getByPage(1);
        return `

            <m-flex data-type="col" class="w" id="flx${_self.name}s">

                <m-flex data-type="row" class="n">

                    <h1 class="w">Phases</h1>

                    <m-flex data-type="row" class="n c sm sQ tertiary btnOpenModule" data-function="ProjectPhase.getHtmlModuleAdd" data-args="">
                        <i class="icon-plus"><svg><use xlink:href="/Content/Images/Bambino.min.svg#icon-plus"></use></svg></i>
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
            <m-flex data-type="col" class="n s cards selectable" id="lst${_self.name}s">

                <m-flex data-type="row" class="tableRow n pL pR mB sC sort" style="width: 100%;">
                    <h2 class="${Global.getSort(_self.sort, `name`)}" data-sort="name">
                        Name
                    </h2>
                    <h2 class="${Global.getSort(_self.sort, `dateStart`)}" data-sort="dateStart">
                        Start
                    </h2>
                    <h2 class="${Global.getSort(_self.sort, `dateEnd`)}" data-sort="dateEnd">
                        End
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
                    <h1>${_self.name}</h1>
                    <label>Settings</label>
                </m-flex>

                <m-flex data-type="row" class="n c sm sQ secondary btnOpenModule" data-function="Module.getHtmlConfirmation" data-args="phase,btnDelete${_self.name}">
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
                        <label for="txtName${_self.name}">Name</label>
                        <input type="text" id="txtName${_self.name}" placeholder="Name" value="${_self.vm.name}" />
                    </m-input>
                </m-flex>

                <m-flex data-type="row" class="n">
                    <m-input class="n mR">
                        <label for="txtDescription${_self.name}">Description</label>
                        <textarea type="text" id="txtDescription${_self.name}" placeholder="Description">${_self.vm.description}</textarea>
                    </m-input>
                </m-flex>

                <m-flex data-type="row" class="n">
                    <m-input class="n mR">
                        <label for="dteDateStart${_self.name}">Start Date</label>
                        <input type="date" id="dteDateStart${_self.name}" placeholder="" value="${moment(_self.vm.dateStart).format(`YYYY-MM-DD`)}" />
                    </m-input>

                    <m-input class="n mR">
                        <label for="dteDateEnd${_self.name}">End Date</label>
                        <input type="date" id="dteDateEnd${_self.name}" placeholder="" value="${moment(_self.vm.dateEnd).format(`YYYY-MM-DD`)}" />
                    </m-input>
                </m-flex>

            </m-flex>

            `;
    };

    const getHtmlCard = function (obj) {
        return `

            <m-card class="tableRow mB nT btnOpenModule" data-function="ProjectPhase.getHtmlModuleDetail" data-args="${obj.projectPhaseId}">
                <m-flex data-type="row" class="n pL sC tE">
                    <h2 class="tE">
                        ${obj.name}
                    </h2>
                    <h2 class="tE">
                        ${moment(obj.dateStart).format(`MMM, Do YYYY`)}
                    </h2>
                    <h2 class="tE">
                        ${moment(obj.dateEnd).format(`MMM, Do YYYY`)}
                    </h2>
                    ${Global.jack.mIM ? `
                    <m-flex data-type="row" class="n c sm sQ tertiary btnOpenModule" data-function="Module.getHtmlConfirmation" data-args="phase,btnDelete${_self.name}">
                        <i class="icon-trash-can"><svg><use xlink:href="/Content/Images/Bambino.min.svg#icon-trash-can"></use></svg></i>
                    </m-flex>` : ``}
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
    })();

    return {
        getSelf: getSelf,
        getHtmlModuleAdd: getHtmlModuleAdd,
        getHtmlModuleDetail: getHtmlModuleDetail,
        getHtmlHeader: getHtmlHeader,
        getHtmlBody: getHtmlBody,
        getHtmlBodyList: getHtmlBodyList,
        getHtmlBodyDetail: getHtmlBodyDetail,
        getHtmlBodyForm: getHtmlBodyForm,
        getHtmlCard: getHtmlCard
    }

})();
const ProjectMember = (function () {

    //Private ------------------------------------------------
    let _self = {
        timeout: undefined,
        records: 10,
        page: 1,
        sort: `email asc`,
        isShowMore: false,
        vm: {},
        arr: [],
        name: `ProjectMember`
    };

    const _add = function () {
        
        const vm = {
            projectId: Project.getSelf().vm.projectId,
            email: $(`#txtSearch${_self.name}`).val()
        };

        Global.post(`Project_AddMember`, vm)
            .done(function (data) {

                $(`#flx${_self.name}s`).append(getHtmlCard(data));
                $(`#txtSearch${_self.name}`).val(``);

                Validation.notification(1);
            }).fail(function (data) {
                Validation.notification(1, `Error`, data.responseJSON.Message, `error`);
            });

    };

    const _delete = function ($this) {

        const vm = {
            projectId: Project.getSelf().vm.projectId,
            token: $this.attr(`data-id`)
        };

        Global.post(`Project_DeleteMember`, vm)
            .done(function (data) {

                $(`m-card[data-id="${vm.token}"]`).remove();

                Validation.notification(1);
            }).fail(function (data) {
                Validation.notification(1, `Error`, data.responseJSON.Message, `error`);
            });

    };

    const _getByPage = function (page) {

        let options = ``;
        const vm = {
            page: page,
            records: _self.records,
            search: ($(`#txtSearch${_self.name}`).val()) ? $(`#txtSearch${_self.name}`).val() : ``,
            sort: _self.sort
        }

        if (page == 1) _self.arr = [];

        //$(`m-body[data-label="Primary"] .flxLoading, m-body[data-label="Primary"] #lst${_self.name}s`).remove();
        //$(`m-body[data-label="Primary"] #flx${_self.name}s`).append(Global.getHtmlLoading());

        _self.page = page;
        Global.post(`${_self.name}_GetByPage`, vm)
            .done(function (data) {

                _self.isShowMore = true;
                if (data.totalRecords < (vm.page * vm.records))
                    _self.isShowMore = false;

                for (let obj of data.arr)
                    _self.arr.push(obj);
                
                $(`m-select[data-name="${_self.name}"]`).remove();

                if ($(`#txtSearch${_self.name}`).val() == ``)
                    return;

                for (let obj of _self.arr)
                    options += `<m-option data-name="${_self.name}">${obj.email}</m-option>`;

                $(`#txtSearch${_self.name}`).parent().append(`<m-select data-name="${_self.name}">${options}</m-select>`);

            })
            .fail(function (data) {
                Validation.notification(2);
            });

    };

    const _search = function (e, $this) {

        if (e.which == 13) {

            $(`m-select[data-name="${_self.name}"]`).remove();
            _add();

            return;

        }

        _getByPage(1);

    };
    const _select = function ($this) {

        $(`#txtSearch${_self.name}`).val($this.html()).focus();
        $(`m-select[data-name="${_self.name}"]`).remove();

    };
    //Public -------------------------------------------------

    const getHtmlBody = function () {

        let html = ``;

        for (let obj of Project.getSelf().vm.members)
            html += getHtmlCard(obj);

        return `

            <m-flex data-type="col" class="w n" id="flx${_self.name}s">

                <m-flex data-type="row" class="n">

                    <h1 class="w">Members</h1>

                    <m-flex data-type="row" class="w n">

                        <m-input class="n">
                            <input type="text" id="txtSearch${_self.name}" placeholder="Search" value="" required />
                        </m-input>

                    </m-flex>

                </m-flex>

                <m-flex data-type="col" class="w n pT" id="">
                    ${html}
                </m-flex>

            </m-flex>

            `;
    };

    const getHtmlCard = function (obj) {
        return `

            <m-card class="tableRow mB nT" data-id="${obj.token}">
                <m-flex data-type="row" class="n pL sC tE">
                    <m-dot class="${obj.isActive ? `g` : `r`}"></m-dot>
                    <h2 class="tE">
                        ${obj.email}
                    </h2>
                    ${Global.jack.mIM ? `
                    <m-flex data-type="row" class="n c sm sQ tertiary btnDeleteMember" data-id="${obj.token}">
                        <i class="icon-trash-can"><svg><use xlink:href="/Content/Images/Bambino.min.svg#icon-trash-can"></use></svg></i>
                    </m-flex>` : ``}
                </m-flex>
            </m-card>

            `;
    };

    const _init = (function () {
        $(document).on(`tap`, `.btnDeleteMember`, function () { _delete($(this)); });
        $(document).on(`tap`, `m-option[data-name="${_self.name}"]`, function () { _select($(this)); });
        $(document).on(`keyup`, `#txtSearch${_self.name}`, function (e) { _search(e, $(this)); });
    })();

    return {
        getHtmlBody: getHtmlBody,
        getHtmlCard: getHtmlCard
    };

})();