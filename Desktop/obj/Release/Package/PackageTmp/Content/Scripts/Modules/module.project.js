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
        history: {
            fn: ``,
            args: ``
        },
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

                        <m-button data-type="secondary" class="sQ btnOpenBody mR" data-label="Project" data-function="ProjectAttraction.getHtmlBody" data-args="">
                            Attractions
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
        pageArr: [],
        sort: `code asc`,
        isShowMore: false,
        name: `ProjectZone`,
        arr: [],
        vm: {},
        constructor: function (projectZoneKey, projectId, name, description, code, isArchived, isDeleted) {
            this.projectZoneKey = projectZoneKey;
            this.projectId = projectId;
            this.name = name;
            this.description = description;
            this.code = code;
            this.isArchived = isArchived;
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

                    _self.pageArr = [];

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

    const _get = function () {

        if (_self.arr.length != 0)
            return;

        const vm = {
            id: Project.getSelf().vm.projectId
        };
        
        Global.post(`${_self.name}_Get`, vm)
            .done(function (data) {
                _self.arr = data;
            })
            .fail(function (data) {
                Validation.notification(2);
            });

    };
    const _getByPage = function (page) {

        const vm = {
            page: page,
            records: _self.records,
            search: ($(`#txtSearch${_self.name}`).val()) ? $(`#txtSearch${_self.name}`).val() : ``,
            sort: _self.sort,
            id: Project.getSelf().vm.projectId,
            projectPhaseKey: Project.getSelf().vm.projectPhaseKey
        }

        if (page == 1) _self.pageArr = [];

        $(`m-body[data-label="Primary"] .flxLoading, m-body[data-label="Primary"] #lst${_self.name}s`).remove();
        $(`m-body[data-label="Primary"] #flx${_self.name}s`).append(Global.getHtmlLoading());

        Project.getSelf().history.fn = `ProjectZone.getByPage`;
        Project.getSelf().history.args = `1`;

        _self.page = page;
        Global.post(`${_self.name}_GetByPage`, vm)
            .done(function (data) {

                _self.isShowMore = true;
                if (data.totalRecords < (vm.page * vm.records))
                    _self.isShowMore = false;

                for (let obj of data.arr)
                    _self.pageArr.push(obj);

                $(`m-body[data-label="Primary"] .flxLoading, m-body[data-label="Primary"] #lst${_self.name}s`).remove();
                $(`m-body[data-label="Primary"] #flx${_self.name}s`).append(getHtmlBodyList(_self.pageArr));

            })
            .fail(function (data) {
                Validation.notification(2);
            });

    };
    const _getByKey = function (key) {

        const vm = {
            key: key
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
        return new _self.constructor(0, Global.guidEmpty, ``, ``, 0, false, false);
    };

    const _search = function () {

        clearTimeout(_self.timeout);
        _self.timeout = setTimeout(function () {
            _getByPage(1);
        }, Global.keyUpTimeout);

    };
    const _sort = function ($this) {

        const dir = ($this.hasClass(`sortasc`)) ? `desc` : `asc`;

        _self.sort = `${$this.attr(`data-sort`)} ${dir}`;
        $(`.sort h2`).removeClass(`sortasc`).removeClass(`sortdesc`);
        $this.addClass(`sort${dir}`);
        _getByPage(1);

    };
    const _archive = function ($this, isArchived) {

        const vm = {
            key: $this.attr(`data-id`),
            isArchived: isArchived
        };

        Global.post(`${_self.name}_EditArchive`, vm)
            .done(function (data) {

                _self.pageArr = [];

                _getByPage(1);

                Validation.notification(1);
                Module.closeModuleAll();
            })
            .fail(function (data) {
                Validation.notification(2);
            });

    };

    //Public ------------------------------------------------
    const get = function () {
        _get();
    };
    const getSelf = function () {
        return _self;
    };
    const getByPage = function (page) {
        return _getByPage(page);
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
    const getHtmlModuleDetail = function (key) {
        _getByKey(key);
        return `

            <m-header data-label="${_self.name} Detail Header">

                <m-flex data-type="row" class="n">
                    <m-flex data-type="row" class="n c tab h btnOpenBody" data-label="${_self.name} Detail Body" data-function="${_self.name}.getHtmlBodyDetail">
                        <span>Information</span>
                    </m-flex>
                </m-flex>

                <!--<m-flex data-type="row" class="n c sQ h btnCloseModule">
                    <i class="icon-delete"><svg><use xlink:href="/Content/Images/Bambino.min.svg#icon-delete"></use></svg></i>
                </m-flex>-->

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

            <m-card class="tableRow mB nT btnOpenModule ${obj.isArchived ? `archive` : ``}" data-function="${_self.name}.getHtmlModuleDetail" data-args="${obj.projectZoneKey}">
                <m-flex data-type="row" class="n pL sC tE">
                    <h2 class="tE">
                        ${Global.getCode(obj.code)}
                    </h2>
                    <h2 class="tE">
                        ${obj.name}
                    </h2>
                    ${Global.jack.mIM && !obj.isArchived ? `
                    <m-flex data-type="row" class="n c sm sQ tertiary btnOpenModule" data-function="Module.getHtmlConfirmation" data-args="Are you sure you want to archive this zone?,btnArchive${_self.name},${obj.projectZoneKey}">
                        <i class="icon-downloads"><svg><use xlink:href="/Content/Images/Bambino.min.svg#icon-downloads"></use></svg></i>
                    </m-flex>` : ``}
                    ${Global.jack.mIM && obj.isArchived ? `
                    <m-flex data-type="row" class="n c sm sQ tertiary btnOpenModule" data-function="Module.getHtmlConfirmation" data-args="Are you sure you want to unarchive this zone?,btnUnarchive${_self.name},${obj.projectZoneKey}">
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
        get: get,
        getSelf: getSelf,
        getByPage: getByPage,
        getHtmlModuleAdd: getHtmlModuleAdd,
        getHtmlModuleDetail: getHtmlModuleDetail,
        getHtmlBody: getHtmlBody,
        getHtmlBodyList: getHtmlBodyList,
        getHtmlBodyDetail: getHtmlBodyDetail,
        getHtmlBodyForm: getHtmlBodyForm,
        getHtmlCard: getHtmlCard
    };

})();
const ProjectAttraction = (function () {

    //Private ------------------------------------------------
    let _self = {
        timeout: undefined,
        records: 100,
        page: 1,
        sort: `code asc`,
        isShowMore: false,
        name: `ProjectAttraction`,
        arr: [],
        vm: {},
        constructor: function (projectAttractionKey, projectZoneKey, name, description, code, isArchived, isDeleted) {
            this.projectAttractionKey = projectAttractionKey;
            this.projectZoneKey = projectZoneKey;
            this.name = name;
            this.description = description;
            this.code = code;
            this.isArchived = isArchived;
            this.isDeleted = isDeleted;
        }
    };

    const _addEdit = function () {

        _self.vm.projectZoneKey = $(`#dboProjectZoneId${_self.name}`).val();
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

                    if (data.state == `add`)
                        _getByPage(1);
                    else
                        $(`m-body[data-label="${_self.name} Detail Body"]`).html(getHtmlBodyDetail());

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
            projectPhaseKey: Project.getSelf().vm.projectPhaseKey
        };

        if (page == 1) _self.arr = [];

        $(`m-body[data-label="Primary"] .flxLoading, m-body[data-label="Primary"] #lst${_self.name}s`).remove();
        $(`m-body[data-label="Primary"] #flx${_self.name}s`).append(Global.getHtmlLoading());

        Project.getSelf().history.fn = `ProjectAttraction.getByPage`;
        Project.getSelf().history.args = `1`;

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
    const _getByKey = function (key) {

        const vm = {
            key: key
        }

        Project.getSelf().history.fn = `ProjectAttraction.getByKey`;
        Project.getSelf().history.args = key;

        Global.post(`${_self.name}_GetById`, vm)
            .done(function (data) {
                console.log(data);

                _self.vm = data;

                if ($(`m-module`).length > 0)
                    $(`m-module m-body`).html(getHtmlBodyEdit());
                else
                    $(`m-body[data-label="${_self.name} Detail Body"]`).html(getHtmlBodyDetail());

            }).fail(function (data) {
                Validation.notification(2);
            });

    };

    const _getEmptyVM = function () {
        return new _self.constructor(0, 0, ``, ``, 0, false, false);
    };

    const _search = function () {

        clearTimeout(_self.timeout);
        _self.timeout = setTimeout(function () {
            _getByPage(1);
        }, Global.keyUpTimeout);

    };
    const _sort = function ($this) {

        const dir = ($this.hasClass(`sortasc`)) ? `desc` : `asc`;

        _self.sort = `${$this.attr(`data-sort`)} ${dir}`;
        $(`.sort h2`).removeClass(`sortasc`).removeClass(`sortdesc`);
        $this.addClass(`sort${dir}`);
        _getByPage(1);

    };
    const _archive = function ($this, isArchived) {

        const vm = {
            key: $this.attr(`data-id`),
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
    const getByPage = function (page) {
        return _getByPage(page);
    };
    const getByKey = function (key) {
        return _getByKey(key);
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
                        <h1>Project Attraction</h1>
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
    const getHtmlModuleDetail = function (key) {
        _getByKey(key);
        return `

            <m-header data-label="${_self.name} Detail Header">

                <m-flex data-type="row" class="n">
                    <m-flex data-type="row" class="n c tab h btnOpenBody" data-label="${_self.name} Detail Body" data-function="${_self.name}.getHtmlBodyDetail">
                        <span>Information</span>
                    </m-flex>
                </m-flex>

                <!--<m-flex data-type="row" class="n c sQ h btnCloseModule">
                    <i class="icon-delete"><svg><use xlink:href="/Content/Images/Bambino.min.svg#icon-delete"></use></svg></i>
                </m-flex>-->

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
        ProjectZone.get();
        return `

            <m-flex data-type="col" class="n w" id="flx${_self.name}s">

                <m-flex data-type="row" class="n">

                    <h1 class="w">Attractions</h1>

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
                    <h2 class="${Global.getSort(_self.sort, `ProjectZone.name`)}" data-sort="ProjectZone.name">
                        Zone
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
    const getHtmlBodyLoading = function (key) {
        _getByKey(key);
        return `

            <m-body data-label="${_self.name} Detail Body">

                <m-flex data-type="col">

                    <h1 class="loading">Loading</h1>
                
                </m-flex>

            </m-body>

            `;
    };
    const getHtmlBodyDetail = function () {
        return `

            <m-flex data-type="row" class="s w n">

                <m-flex data-type="col" class="w25 n pR">

                    <m-card class="crdInfo">
                        <m-flex data-type="col" class="">

                            <h6>Name</h6>

                            <m-flex data-type="row" class="n mB">
                                <h5>${Global.getCode(_self.vm.code)} ${_self.vm.name}</h5>
                            </m-flex>

                            <h6>Description</h6>

                            <m-flex data-type="row" class="n mB">
                                <h5>${_self.vm.description}</h5>
                            </m-flex>

                            <m-flex data-type="row" class="n s mB">

                                <h6 class="mR">${_self.vm.numOfElements}</h6>
                                <m-flex data-type="row" class="n c xs sQ mR tertiary">
                                    <i class="icon-content"><svg><use xlink:href="/Content/Images/Bambino.min.svg#icon-content"></use></svg></i>
                                </m-flex>

                                <h6 class="mR">${_self.vm.numOfReferences}</h6>
                                <m-flex data-type="row" class="n c xs sQ mR tertiary">
                                    <i class="icon-picture"><svg><use xlink:href="/Content/Images/Bambino.min.svg#icon-picture"></use></svg></i>
                                </m-flex>

                            </m-flex>

                            <m-flex data-type="row" class="n mT">

                                <m-flex data-type="row" class="n c xs sQ mR tertiary btnOpenModule" data-function="${_self.name}.getHtmlModuleDetail" data-args="${_self.vm.projectAttractionKey}">
                                    <i class="icon-edit"><svg><use xlink:href="/Content/Images/Bambino.min.svg#icon-edit"></use></svg></i>
                                </m-flex>

                                <m-flex data-type="row" class="n c xs sQ tertiary">
                                    <i class="icon-downloads"><svg><use xlink:href="/Content/Images/Bambino.min.svg#icon-downloads"></use></svg></i>
                                </m-flex>

                            </m-flex>

                        </m-flex>
                    </m-card>

                </m-flex>

                <m-flex data-type="col" class="w75 n">

                    ${ProjectReference.getHtmlBody()}

                </m-flex>

            </m-flex>

            `;
    };
    const getHtmlBodyEdit = function () {
        return `

            <m-flex data-type="row" class="n pL pR">

                <m-flex data-type="col" class="n">
                    <h1>${_self.name}</h1>
                    <label>Settings</label>
                </m-flex>

                <m-flex data-type="row" class="n c sm sQ secondary btnOpenModule" data-function="Module.getHtmlConfirmation" data-args="attraction,btnDelete${_self.name}">
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

                <m-input>
                    <label for="dboProjectZoneId${_self.name}">Zone</label>
                    <select id="dboProjectZoneId${_self.name}">
                        ${Global.getHtmlOptions(ProjectZone.getSelf().arr, [_self.vm.projectZoneKey])}
                    </select>
                </m-input>

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

            <m-card class="tableRow mB nT btnOpenBody ${obj.isArchived ? `archive` : ``}" data-label="Project" data-function="${_self.name}.getHtmlBodyLoading" data-args="${obj.projectAttractionKey}">
                <m-flex data-type="row" class="n pL sC tE">
                    <h2 class="tE">
                        ${Global.getCode(obj.code)}
                    </h2>
                    <h2 class="tE">
                        ${obj.projectZone.name}
                    </h2>
                    <h2 class="tE">
                        ${obj.name}
                    </h2>
                    ${Global.jack.mIM && !obj.isArchived ? `
                    <m-flex data-type="row" class="n c sm sQ tertiary btnOpenModule" data-function="Module.getHtmlConfirmation" data-args="Are you sure you want to archive this attraction?,btnArchive${_self.name},${obj.projectAttractionKey}">
                        <i class="icon-downloads"><svg><use xlink:href="/Content/Images/Bambino.min.svg#icon-downloads"></use></svg></i>
                    </m-flex>` : ``}
                    ${Global.jack.mIM && obj.isArchived ? `
                    <m-flex data-type="row" class="n c sm sQ tertiary btnOpenModule" data-function="Module.getHtmlConfirmation" data-args="Are you sure you want to unarchive this attraction?,btnUnarchive${_self.name},${obj.projectAttractionKey}">
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
        getByPage: getByPage,
        getByKey: getByKey,
        getHtmlModuleAdd: getHtmlModuleAdd,
        getHtmlModuleDetail: getHtmlModuleDetail,
        getHtmlBody: getHtmlBody,
        getHtmlBodyList: getHtmlBodyList,
        getHtmlBodyLoading: getHtmlBodyLoading,
        getHtmlBodyDetail: getHtmlBodyDetail,
        getHtmlBodyEdit: getHtmlBodyEdit,
        getHtmlBodyForm: getHtmlBodyForm,
        getHtmlCard: getHtmlCard
    };

})();
const ProjectReference = (function () {

    //Private ------------------------------------------------
    let _self = {
        timeout: undefined,
        records: 100,
        page: 1,
        sort: `code asc`,
        isShowMore: false,
        name: `ProjectReference`,
        arr: [],
        vm: {},
        constructor: function (projectReferenceKey, projectAttractionKey, disciplineKey, name,
            description, code, path, originalFileName, dateCreated, isArchived, isDeleted, tags) {
            this.projectReferenceKey = projectReferenceKey;
            this.projectAttractionKey = projectAttractionKey;
            this.disciplineKey = disciplineKey;
            this.name = name;
            this.description = description;
            this.code = code;
            this.path = path
            this.originalFileName = originalFileName;
            this.dateCreated = dateCreated;
            this.isArchived = isArchived;
            this.isDeleted = isDeleted;
            this.tags = tags;
        }
    };

    const _addEdit = function () {

        _self.vm.projectAttractionKey = ProjectAttraction.getSelf().vm.projectAttractionKey;
        _self.vm.disciplineKey = $(`#dboDisciplineKey${_self.name}`).val();
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
            key: ProjectAttraction.getSelf().vm.projectAttractionKey,
            projectPhaseKey: Project.getSelf().vm.projectPhaseKey
        };

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
    const _getByKey = function (key) {

        const vm = {
            key: key
        }

        Global.post(`${_self.name}_GetById`, vm)
            .done(function (data) {
                console.log(data);

                _self.vm = data;

                $(`m-module m-body`).html(getHtmlBodyEdit());

            }).fail(function (data) {
                Validation.notification(2);
            });

    };

    const _getEmptyVM = function () {
        return new _self.constructor(0, 0, 0, ``, ``, 0, ``, ``, moment().format(`YYYY-MM-DD`), false, false, []);
    };

    const _upload = function (files) {

        if (files.length == 0 || $(`#btnUpload${_self.name}`).hasClass('disabled')) { _uploadReset(); return; }
        $(`#btnUpload${_self.name}`).addClass('disabled');

        let formData = new FormData();

        for (let i = 0; i < files.length; i++) {
            console.log(Global.getIsValidFile(files[i])); //10MB
            if (files[i].size > 10000000) { Validation.notification(1, `File Upload Limit`, `File size is above 10MB in size, please reduce the file size before uploading.`, `error`); _uploadReset(); return; }
            if (Global.getIsValidFile(files[i]) == false) { Validation.notification(1, `File Upload Type`, `File extension is not allowed.`, `error`); _uploadReset(); return; }
            formData.append(files[i].name, files[i]);
        }

        console.log(`uploadSuccess`);
        Global.upload(formData, ProjectReference.uploadSuccess, '/File/Upload');

    };
    const _uploadReset = function () {
        $(`#upl${_self.name}`).val(``);
        $(`#btnUpload${_self.name}`).removeClass(`disabled`);
    };
    const _search = function () {

        clearTimeout(_self.timeout);
        _self.timeout = setTimeout(function () {
            _getByPage(1);
        }, Global.keyUpTimeout);

    };
    const _sort = function ($this) {

        const dir = ($this.hasClass(`sortasc`)) ? `desc` : `asc`;

        _self.sort = `${$this.attr(`data-sort`)} ${dir}`;
        $(`.sort h2`).removeClass(`sortasc`).removeClass(`sortdesc`);
        $this.addClass(`sort${dir}`);
        _getByPage(1);

    };
    const _archive = function ($this, isArchived) {

        const vm = {
            key: $this.attr(`data-id`),
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
    const getByPage = function (page) {
        return _getByPage(page);
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
                        <h1>Reference</h1>
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
    const getHtmlModuleDetail = function (key) {
        _getByKey(key);
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

                    <h1 class="w">References</h1>

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
            <m-flex data-type="col" class="n s cards selectable wR" id="lst${_self.name}s">

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
    const getHtmlBodyLoading = function (key) {
        _getByKey(key);
        return `

            <m-body data-label="${_self.name} Detail Body">

                <m-flex data-type="col">

                    <h1 class="loading">Loading</h1>
                
                </m-flex>

            </m-body>

            `;
    };
    const getHtmlBodyDetail = function () {
        return `

            <m-flex data-type="row" class="s w n">


            </m-flex>

            `;
    };
    const getHtmlBodyEdit = function () {
        return `

            <m-flex data-type="row" class="n pL pR">

                <m-flex data-type="col" class="n">
                    <h1>${_self.name}</h1>
                    <label>Settings</label>
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

        let html = ``;

        for (let tag of _self.vm.tags)
            html += ProjectReferenceTag.getHtmlTag(tag);

        return `

            <m-flex data-type="col" class="form">

               ${_self.vm.projectReferenceKey == 0 ? `` : `
                <m-image id="imgPath${_self.name}" class="cover" style="height: 100px;background-image: url(${_self.vm.path});">
                </m-image>

                <m-flex data-type="row" class="n">

                    <m-button data-type="primary" class="" id="btnUpload${_self.name}">
                        Upload
                    </m-button>
                    <input type="file" class="none" id="upl${_self.name}" />

                </m-flex>`}

                <m-flex data-type="row" class="n">
                    <m-input class="mR">
                        <label for="txtName${_self.name}">Name</label>
                        <input type="text" id="txtName${_self.name}" placeholder="Name" value="${_self.vm.name}" />
                    </m-input>

                    <m-input>
                        <label for="dboDisciplineKey${_self.name}">Discipline</label>
                        <select id="dboDisciplineKey${_self.name}">
                            ${Global.getHtmlOptions(Disciplines.getSelf().arr, [_self.vm.disciplineKey])}
                        </select>
                    </m-input>
                </m-flex>

                <m-input class="mR">
                    <label for="txtDescription${_self.name}">Description</label>
                    <input type="text" id="txtDescription${_self.name}" placeholder="Description" value="${_self.vm.description}" />
                </m-input>

               ${_self.vm.projectReferenceKey == 0 ? `` : `
                <m-flex data-type="row" class="n s">
                    <m-input class="mR">
                        <input type="text" id="txtSearchProjectReferenceTag" placeholder="Tags" value="" />
                    </m-input>

                    <m-flex data-type="row" class="n c sm sQ secondary btnAddProjectReferenceTag">
                        <i class="icon-plus"><svg><use xlink:href="/Content/Images/Bambino.min.svg#icon-plus"></use></svg></i>
                    </m-flex>
                </m-flex>

                <label for="txtSearchProjectReferenceTag" class="mB">Reference Tags</label>

                <m-flex data-type="row" class="n s" id="flxProjectReferenceTags">
                    ${html}
                </m-flex>`}

            </m-flex>

            `;
    };

    const getHtmlCard = function (obj) {
        return `

            <m-card class="tableRow mB mR w20 nT btnOpenModule ${obj.isArchived ? `archive` : ``}" data-function="${_self.name}.getHtmlModuleDetail" data-args="${obj.projectReferenceKey}">
                <m-image class="sQ cover" style="background-image: url(${obj.path});">
                </m-image>
                <m-flex data-type="row" class="">
                    <m-flex data-type="col" class="n sC tE">
                        <h2 class="tE">
                            ${obj.name}
                        </h2>
                        <h6 class="tE">
                            ${Global.getCode(obj.code)}
                        </h6>
                    </m-flex>
                    ${Global.jack.mIM && !obj.isArchived ? `
                    <m-flex data-type="row" class="n c sm sQ tertiary btnOpenModule" data-function="Module.getHtmlConfirmation" data-args="Are you sure you want to archive this reference?,btnArchive${_self.name},${obj.projectReferenceKey}">
                        <i class="icon-downloads"><svg><use xlink:href="/Content/Images/Bambino.min.svg#icon-downloads"></use></svg></i>
                    </m-flex>` : ``}
                    ${Global.jack.mIM && obj.isArchived ? `
                    <m-flex data-type="row" class="n c sm sQ tertiary btnOpenModule" data-function="Module.getHtmlConfirmation" data-args="Are you sure you want to unarchive this reference?,btnUnarchive${_self.name},${obj.projectReferenceKey}">
                        <i class="icon-downloads"><svg><use xlink:href="/Content/Images/Bambino.min.svg#icon-downloads"></use></svg></i>
                    </m-flex>` : ``}
                </m-flex>
            </m-card>

            `;
    };

    const uploadSuccess = function (arr) {

        for (let obj of arr) {
            _self.vm.path = obj.path;
            _self.vm.originalFileName = obj.originalFileName;
        }

        $(`#imgPath${_self.name}`).css(`background-image`, `url(${_self.vm.path})`);
        //_addEditDelete();
        _uploadReset();

    };

    const _init = (function () {
        $(document).on(`tap`, `#btnUpload${_self.name}`, function (e) { e.stopPropagation(); e.preventDefault(); $(`#upl${_self.name}`).click(); });
        $(document).on(`change`, `#upl${_self.name}`, function () { _upload($(this).prop(`files`)); });
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
        getByPage: getByPage,
        getHtmlModuleAdd: getHtmlModuleAdd,
        getHtmlModuleDetail: getHtmlModuleDetail,
        getHtmlBody: getHtmlBody,
        getHtmlBodyList: getHtmlBodyList,
        getHtmlBodyLoading: getHtmlBodyLoading,
        getHtmlBodyDetail: getHtmlBodyDetail,
        getHtmlBodyEdit: getHtmlBodyEdit,
        getHtmlBodyForm: getHtmlBodyForm,
        getHtmlCard: getHtmlCard,
        uploadSuccess: uploadSuccess
    };

})();
const ProjectReferenceTag = (function () {

    //Private -----------------------------------------
    let _self = {
        arr: [],
        vm: {},
        name: `ProjectReferenceTag`,
        timeout: undefined
    };

    const _add = function () {

        const vm = {
            tableKey: ProjectReference.getSelf().vm.projectReferenceKey,
            name: $(`#txtSearch${_self.name}`).val()
        };

        Global.post(`ProjectReference_AddTag`, vm)
            .done(function (data) {

                $(`#flx${_self.name}s`).append(getHtmlTag(data));
                $(`#txtSearch${_self.name}`).val(``);

                Validation.notification(1);
            }).fail(function (data) {
                Validation.notification(1, `Error`, data.responseJSON.Message, `error`);
            });

    }

    const _delete = function ($this) {

        const vm = {
            tableKey: ProjectReference.getSelf().vm.projectReferenceKey,
            manyKey: $this.attr(`data-id`)
        };

        Global.post(`ProjectReference_DeleteTag`, vm)
            .done(function (data) {

                $(`m-card[data-id="${data.projectReferenceTagKey}"]`).remove();

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

            <m-card class="tag" data-id="${obj.projectReferenceTagKey}">
                <m-flex data-type="row" class="n">
                    <h1 class="tE">
                        ${obj.name}
                    </h1>
                    <m-flex data-type="row" class="n c xs sQ tertiary btnDelete${_self.name}" data-id="${obj.projectReferenceTagKey}">
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
        constructor: function (projectPhaseKey, projectId, name, description, sortOrder, dateStart, dateEnd, isDeleted) {
            this.projectPhaseKey = projectPhaseKey;
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
    const _getByKey = function (key) {

        const vm = {
            key: key
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
        return new _self.constructor(0, Global.guidEmpty, ``, ``, 0, ``, ``, false);
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
    const _select = function ($this) {

        $(`.btnSelect${_self.name}`).removeClass(`active`);
        $this.addClass(`active`);

        Project.getSelf().vm.projectPhaseKey = $this.attr(`data-id`);
        Global.getFunctionByName(Project.getSelf().history.fn, Project.getSelf().history.args);

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
    const getHtmlModuleDetail = function (key) {
        _getByKey(key);
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
            html += `<span data-id="${obj.projectPhaseKey}" class="btnSelect${_self.name} ${Project.getSelf().vm.projectPhaseKey == obj.projectPhaseKey ? `active` : ``}">${obj.name.match(/\b(\w)/g).join('').toUpperCase()}</span>`;

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

            <m-card class="tableRow mB nT btnOpenModule" data-function="ProjectPhase.getHtmlModuleDetail" data-args="${obj.projectPhaseKey}">
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
        $(document).on(`tap`, `.btnSelect${_self.name}`, function () { _select($(this)); });
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
                    ${Members.getHtmlIcon(obj)}
                    <h2 class="tE mL">
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