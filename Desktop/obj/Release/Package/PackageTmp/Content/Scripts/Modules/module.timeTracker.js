'use strict';

const TimeTracker = (function () {

    //Private ------------------------------------------------
    let _self = {
        vm: {},
        arr: [],
        name: `TimeTracker`,
        totalHours: 0,
        currentHours: 0
    };

    const _in = function () {

        Global.post(`${_self.name}_In`, {})
            .done(function (data) {

                $(`m-timetracker`).attr(`data-isActive`, `true`);
                Global.editIcon($(`m-timetracker`),`icon-clock`);

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

    const _getIsActive = function () {

        Global.post(`${_self.name}_GetIsActive`, {})
            .done(function (data) {

                $(`body`).append(`${_getHtmlIcon(data)}`);

            }).fail(function (data) {
                Validation.notification(2);
            });

    };
    const _getProjectsByMember = function () {

        Global.post(`ProjectMember_GetProjectsByMember`, {})
            .done(function (data) {
                $(`m-timetrackerprojects`).remove();
                $(`body`).append(_getHtmlCardProjects(data));
            }).fail(function (data) {
                Validation.notification(2);
            });

    };

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

    //Public ------------------------------------------------
    const getIsActive = function () {
        _getIsActive();
    };
    const getSelf = function () {
        return _self;
    };

    const _init = (function () {
        //rngTimeTrackerProject
        $(document).on(`input`, `.rngTimeTrackerProject`, function (e) { _editHours(e,$(this)); });
        $(document).on(`tap`, `#btnCloseTimeTrackerProjects`, function () { $(`m-timetrackerprojects`).remove(); });
        $(document).on(`tap`, `m-timetracker[data-isActive="true"]`, function () { _getProjectsByMember(); });
        $(document).on(`tap`, `#btnOut${_self.name}`, function () { _out(); });
        $(document).on(`tap`, `m-timetracker[data-isActive="false"]`, function () { _in(); });
    })();

    return {
        getIsActive: getIsActive,
        getSelf: getSelf
    }

})();