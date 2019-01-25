'use strict';

const TimeTracker = (function () {

    //Private ------------------------------------------------
    let _self = {
        timeout: undefined,
        records: 100,
        page: 1,
        sort: `dateIn desc`,
        isShowMore: false,
        name: `TimeTracker`,
        arr: [],
        vm: {},
        constructor: function (materialId, name, isDeleted) {
            this.materialId = materialId;
            this.name = name;
            this.isDeleted = isDeleted;
        },
        totalHours: 0,
        currentHours: 0,
        id: Global.guidEmpty
    };

    const _in = function () {

        Global.editIcon($(`m-timetracker`), `icon-clock`);

        Global.post(`${_self.name}_In`, {})
            .done(function (data) {

                $(`m-timetracker`).attr(`data-isActive`, `true`);

            }).fail(function (data) {
                Validation.notification(2);
            });

    }
    const _out = function () {

        let vm = {
            totalHours: _self.totalHours,
            projects: []
        };

        $(`.rngTimeTrackerProject`).each(function (index) {

            const hours = parseFloat($(this).val());

            if (hours == 0)
                return;

            vm.projects.push({
                projectId: $(this).attr(`data-id`),
                totalHours: parseFloat($(this).val())
            });

        });

        const currentHours = vm.projects.reduce(function (a, b) { return a + b.totalHours; }, 0);

        if (currentHours != _self.totalHours) {
            //Add other
            vm.projects.push({
                projectId: Global.guidEmpty,
                totalHours: _self.totalHours - currentHours
            });

        }

        console.log(`vm`,vm);
        Global.post(`${_self.name}_Out`, vm)
            .done(function (data) {

                $(`m-timetracker`).attr(`data-isActive`, `false`);
                Global.editIcon($(`m-timetracker`), `icon-expired`);
                $(`m-timetrackerprojects`).remove();

            }).fail(function (data) {
                Validation.notification(2);
            });

    };

    const _editHours = function (e, $this) {

        let otherHours = 0;

        _self.currentHours = 0;
        $(`.rngTimeTrackerProject`).each(function (index) {

            const hours = parseFloat($(this).val());

            _self.currentHours += hours;

            if ($(this).attr(`data-id`) != $this.attr(`data-id`))
                otherHours += hours;

        });
        
        if (_self.currentHours > _self.totalHours) {
            $this.val(_self.totalHours - otherHours);
            $(`#h5CurrentHours`).html(parseFloat(_self.totalHours).toFixed(1));
        } else {
            $(`#h5CurrentHours`).html(parseFloat(_self.currentHours).toFixed(1));
        }

        $(`m-card[data-type="TimeTrackerProject"][data-id="${$this.attr(`data-id`)}"] h5`).html(parseFloat($this.val()).toFixed(1));

    }

    const _getByPage = function (page) {

        const vm = {
            page: page,
            records: _self.records,
            search: ($(`#txtSearch${_self.name}`).val()) ? $(`#txtSearch${_self.name}`).val() : ``,
            sort: _self.sort,
            id: _self.id
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

    }
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
    const _getIsActive = function () {

        Global.post(`${_self.name}_GetIsActive`, {})
            .done(function (data) {

                $(`body`).append(`${_getHtmlIcon(data)}`);

            }).fail(function (data) {
                Validation.notification(2);
            });

    };
    const _getProjectsByMember = function () {

        $(`body`).append(_getHtmlCardProjectsLoading());
        Global.post(`ProjectMember_GetProjectsByMember`, {})
            .done(function (data) {
                $(`m-timetrackerprojects`).remove();
                $(`body`).append(_getHtmlCardProjects(data));
            }).fail(function (data) {
                Validation.notification(2);
            });

    };

    const _getHtmlCardProjectsLoading = function () {
        return `

            <m-timetrackerprojects>

                <m-flex data-type="row" class="">

                    <h1 class="loading">Loading</h1>

                </m-flex>

            </m-timetrackerprojects>

            `;
    }
    const _getHtmlCardProjects = function (data) {

        const duration = moment.duration(moment(data.dateIn).diff(moment()));
        const hours = Math.ceil(Math.abs(duration.asHours()));
        let html = ``;

        for (let obj of data.projects)
            html += _getHtmlCardProject(obj, hours);

        _self.totalHours = hours;

        return `

            <m-timetrackerprojects>

                <m-flex data-type="row" class="n pL pR pT">

                    <h6>${hours} Hours</h6>

                    <m-flex data-type="row" class="n c xs sQ tertiary" id="btnCloseTimeTrackerProjects">
                        <i class="icon-delete"><svg><use xlink:href="/Content/Images/Bambino.min.svg#icon-delete"></use></svg></i>
                    </m-flex>

                </m-flex>

                <m-flex data-type="col" class="n">
                    ${html}
                </m-flex>

                <m-flex data-type="row" class="">

                    <h1 class="tE w50">
                        Total
                    </h1>

                    <m-input class="n mL w">
                    </m-input>

                    <h5 class="w25" id="h5CurrentHours">0.0</h5>

                </m-flex>

                <m-flex data-type="row" class="footer">
                    <m-button data-type="secondary" class="sm" id="btnCloseTimeTrackerProjects">
                        Cancel
                    </m-button>

                    <m-button data-type="primary" class="sm" id="btnOut${_self.name}">
                        Submit
                    </m-button>
                </m-flex>

            </m-timetrackerprojects>

            `;
    }
    const _getHtmlCardProject = function (obj, hours) {
        return `

            <m-card data-type="TimeTrackerProject" class="" data-id="${obj.projectId}">
                <m-flex data-type="row">

                    <h1 class="tE w50">${obj.name}</h1>

                    <m-input class="n mL w">
                        <input type="range" id="rngProject${obj.projectId}" data-id="${obj.projectId}" class="rngTimeTrackerProject" min="0" max="${hours}" value="0" step=".5">
                    </m-input>

                    <h5 class="w25">0.0</h5>

                </m-flex>
            </m-card>

            `;
    }

    const _getHtmlIcon = function (isActive) {
        return `

            <m-timetracker data-isActive="${isActive}">
                <m-flex data-type="row" class="n c sm sQ tertiary">
                    <i class="icon-${isActive ? `clock` : `expired`}"><svg><use xlink:href="/Content/Images/Bambino.min.svg#icon-${isActive ? `clock` : `expired`}"></use></svg></i>
                </m-flex>
            </m-timetracker>

            `;
    }

    const _sort = function ($this) {

        var dir = ($this.hasClass(`sortasc`)) ? `desc` : `asc`;

        _self.sort = `${$this.attr(`data-sort`)} ${dir}`;
        $(`.sort h2`).removeClass(`sortasc`).removeClass(`sortdesc`);
        $this.addClass(`sort${dir}`);
        _getByPage(1);

    }

    //Public ------------------------------------------------
    const getIsActive = function () {
        _getIsActive();
    };
    const getSelf = function () {
        return _self;
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
    }

    const getHtmlBody = function () {
        _self.id = Global.guidEmpty;
        _getByPage(1);
        return `

            <m-flex data-type="col" id="flx${_self.name}s">

                <m-flex data-type="row" class="c n pL pR">

                    <h1 class="w">${_self.name}</h1>

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
                    <h2 class="${Global.getSort(_self.sort, `totalHours`)}" data-sort="totalHours">
                        Total Hours
                    </h2>
                    <h2 class="${Global.getSort(_self.sort, `dateIn`)}" data-sort="dateIn">
                        Time
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
    const getHtmlBodyMember = function () {
        _self.id = Global.jack.mT;
        _getByPage(1);
        return `

            <m-flex data-type="col" id="flx${_self.name}s">

                <m-flex data-type="row" class="c n pL pR">

                    <h1 class="w">${_self.name}</h1>

                </m-flex>
            
                ${Global.getHtmlLoading()}

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

                <m-flex data-type="row" class="n c sm sQ secondary btnOpenModule" data-function="Module.getHtmlConfirmation" data-args="time sheet,btnDelete${_self.name}">
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
        return `

            <m-flex data-type="col" class="form">

                <m-flex data-type="row" class="n">

                    <m-input class="mR">
                        <label for="txtName${_self.name}">Name</label>
                        <input type="text" id="txtName${_self.name}" placeholder="Name" value="${_self.vm.dateIn}" />
                    </m-input>

                    <m-input class="mR">
                        <label for="txtName${_self.name}">Name</label>
                        <input type="text" id="txtName${_self.name}" placeholder="Name" value="${_self.vm.dateOut}" />
                    </m-input>

                </m-flex>

            </m-flex>

            `;
    }

    const getHtmlCard = function (obj) {
        return `

            <m-card class="tableRow mB btnOpenModule" data-function="TimeTracker.getHtmlModuleDetail" data-args="${obj.timeTrackerId}">
                <m-flex data-type="row" class="sC tE">
                    ${obj.isActive ? `<m-dot class="g"></m-dot>` : ``}
                    <h2 class="tE">
                        ${obj.totalHours}
                    </h2>
                    <h2 class="tE">
                        ${moment(obj.dateIn).format(`MMM Do, YYYY hh:mm a`)} - ${moment(obj.dateOut).format(`hh:mm a`)}
                    </h2>
                </m-flex>
            </m-card>

            `;
    }

    const _init = (function () {
        //rngTimeTrackerProject
        $(document).on(`tap`, `#lst${_self.name}s .sort h2`, function () { _sort($(this)); });
        $(document).on(`input`, `.rngTimeTrackerProject`, function (e) { _editHours(e,$(this)); });
        $(document).on(`tap`, `#btnCloseTimeTrackerProjects`, function () { $(`m-timetrackerprojects`).remove(); });
        $(document).on(`tap`, `m-timetracker[data-isActive="true"]`, function () { _getProjectsByMember(); });
        $(document).on(`tap`, `#btnOut${_self.name}`, function () { _out(); });
        $(document).on(`tap`, `m-timetracker[data-isActive="false"]`, function () { _in(); });
    })();

    return {
        getIsActive: getIsActive,
        getSelf: getSelf,
        getHtmlModuleDetail: getHtmlModuleDetail,
        getHtmlBody: getHtmlBody,
        getHtmlBodyList: getHtmlBodyList,
        getHtmlBodyMember: getHtmlBodyMember,
        getHtmlBodyDetail: getHtmlBodyDetail,
        getHtmlBodyForm: getHtmlBodyForm,
        getHtmlCard: getHtmlCard
    }

})();
const TimeTrackerProject = (function () {

    //Private ------------------------------------------------
    let _self = {
        timeout: undefined,
        records: 100,
        page: 1,
        sort: `timeTracker.member.firstName asc`,
        isShowMore: false,
        name: `TimeTrackerProject`,
        arr: [],
        vm: {},
        constructor: function (materialId, name, isDeleted) {
            this.materialId = materialId;
            this.name = name;
            this.isDeleted = isDeleted;
        }
    };

    const _getByPage = function (page, id) {

        const vm = {
            page: page,
            records: _self.records,
            search: ($(`#txtSearch${_self.name}`).val()) ? $(`#txtSearch${_self.name}`).val() : ``,
            sort: _self.sort,
            id: id == `` ? Global.guidEmpty : id 
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
                $(`m-body[data-label="Primary"] #flx${_self.name}s`).append(getHtmlBodyList(_self.arr, data.totalHours));

            })
            .fail(function (data) {
                Validation.notification(2);
            });

    }

    //Public ------------------------------------------------
    const getSelf = function () {
        return _self;
    };

    const getHtmlBody = function (id) {
        _getByPage(1, id);
        return `

            <m-flex data-type="col" id="flx${_self.name}s">

                <m-flex data-type="row" class="c n pL pR">

                    <h1 class="w">${_self.name}</h1>

                </m-flex>
            
                ${Global.getHtmlLoading()}

            </m-flex>

            `;
    }
    const getHtmlBodyList = function (arr, totalHours) {

        let html = ``;

        for (let obj of arr)
            html += getHtmlCard(obj);

        return `

            <m-flex data-type="row" class="s" id="">

                <h1>Total Hours: ${totalHours}</h1>
            
            </m-flex>

            <m-flex data-type="col" class="s cards selectable" id="lst${_self.name}s">

                <m-flex data-type="row" class="tableRow n pL pR mB sC sort" style="width: 100%;">
                    <h2 class="${Global.getSort(_self.sort, `timeTracker.member.firstName`)}" data-sort="firstName">
                        Name
                    </h2>
                    <h2 class="${Global.getSort(_self.sort, `totalHours`)}" data-sort="totalHours">
                        Total Hours
                    </h2>
                    <h2 class="${Global.getSort(_self.sort, `timeTracker.dateIn`)}" data-sort="timeTracker.dateIn">
                        Time
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

    const getHtmlCard = function (obj) {
        return `

            <m-card class="tableRow mB" data-function="TimeTrackerProject.getHtmlModuleDetail" data-args="${obj.timeTrackerProjectId}">
                <m-flex data-type="row" class="sC tE">
                    <h2 class="tE">
                        ${obj.timeTracker.member.firstName} ${obj.timeTracker.member.lastName}
                    </h2>
                    <h2 class="tE">
                        ${obj.totalHours}
                    </h2>
                    <h2 class="tE">
                        ${moment(obj.timeTracker.dateIn).format(`MMM Do, YYYY hh:mm a`)} - ${moment(obj.timeTracker.dateOut).format(`hh:mm a`)}
                    </h2>
                </m-flex>
            </m-card>

            `;
    }

    const _init = (function () {

    })();

    return {
        getSelf: getSelf,
        getHtmlBody: getHtmlBody,
        getHtmlBodyList: getHtmlBodyList,
        getHtmlCard: getHtmlCard
    }

})();