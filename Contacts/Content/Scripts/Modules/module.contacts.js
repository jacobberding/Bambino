'use strict';

const Application = (function () {

    //Private -----------------------------------------------------
    const _open = function () {

        $(`body`).append(Application.getHtmlBody());

    }

    //Public -----------------------------------------------------
    const init = function () {
        _open();
    }

    const getHtmlBody = function () {
        return `

            <m-body data-label="Primary">    
                ${Contacts.getHtmlBody()}
            </m-body>

            `;
    }

    return {
        init: init,
        getHtmlBody: getHtmlBody
    }

})();

const Contacts = (function () {

    //Private ------------------------------------------------
    const _self = {
        timeout: undefined,
        records: 10,
        page: 1,
        arr: [],
        isShowMore: false,
        vm: {},
        name: `Contact`
    }

    const _getByPage = function (page) {

        const vm = {
            page: page,
            records: _self.records,
            search: ($(`#txtSearch${_self.name}`).val()) ? $(`#txtSearch${_self.name}`).val() : ``
        }

        if (page == 1) _self.arr = [];

        _self.page = page;
        Global.post(`Contact_GetByPage`, vm)
            .done(function (data) {

                _self.isShowMore = true;
                if (data.totalRecords < (vm.page * vm.records))
                    _self.isShowMore = false;

                for (let obj of data.arr)
                    _self.arr.push(obj);
                
                $(`m-body[data-label="Primary"] #lst${_self.name}s`).remove();
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

        Global.post(`Contact_GetById`, vm)
            .done(function (data) {
                console.log(data);

                _self.vm = data;

                $(`m-module m-body`).html(getHtmlBodyDetail());

            }).fail(function (data) {
                Validation.notification(2);
            });

    }

    const _search = function () {

        clearTimeout(_self.timeout);
        _self.timeout = setTimeout(function () {
            _getByPage(1);
        }, Global.keyUpTimeout);

    }
    const _upload = function (files) {
        console.log(`_upload`);

        const file = files[0];
        let reader = new FileReader();

        reader.readAsText(file);
        reader.onload = function (event) {

            const data = $.csv.toArrays(event.target.result);
            let vm = {
                contactViewModels: []
            }

            for (var row in data) {

                if (row == 0) continue;
                
                vm.contactViewModels.push({
                    name:                   data[row][0],
                    title:                  data[row][1],
                    companyName:            data[row][2],
                    phone1:                 data[row][3],
                    phone2:                 data[row][4],
                    skypeId:                data[row][5],
                    email:                  data[row][6],
                    companyTemp:            data[row][7],
                    resume:                 data[row][8],
                    portfolio:              data[row][9],
                    personalWebsite:        data[row][10],
                    skills:                 data[row][11],
                    isEdcFamily:            (data[row][12] == `TRUE`) ? true : false,
                    isPotentialStaffing:    (data[row][13] == `TRUE`) ? true : false,
                    //dateCreated:            moment(data[row][14]).format(`YYYY-MM-DD`)
                });

            }

            console.log(`vm`,vm);
            Global.post(`Contact_Upload`, vm)
                .done(function (data) {
                    Validation.notification(1);
                })
                .fail(function (data) {
                    Validation.notification(2);
                });

        };
        reader.onerror = function () { alert('Unable to read ' + file.fileName); };

    }

    //Public ------------------------------------------------
    const getHtmlModuleDetail = function (id) {
        _getById(id);
        return `

            <m-header aria-label="${_self.name} Detail Header">
                <m-flex data-type="row" class="n">
                    <m-flex data-type="row" class="n c tab h btnOpenBody" data-label="${_self.name} Detail Body" data-function="${_self.name}.getHtmlBodyDetail">
                        <span>Information</span>
                    </m-flex>
                </m-flex>
                <m-flex data-type="row" class="n c sQ h btnCloseModule">
                    <i class="icon-delete-3"><svg><use xlink:href="/Content/Images/Ciclops.min.svg#icon-delete-3"></use></svg></i>
                </m-flex>
            </m-header>

            <m-body aria-label="${_self.name} Detail Body">

                <m-flex data-type="col">

                    <h1 class="loading">Loading</h1>
                
                </m-flex>

            </m-body>
        
            `;
    }

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

                        <!--<m-flex data-type="row" class="n c sm sQ secondary btnOpenModule" data-function="Company.getHtmlModuleAdd">
                            <i class="icon-plus"><svg><use xlink:href="/Content/Images/Ciclops.min.svg#icon-plus"></use></svg></i>
                        </m-flex>-->

                        <!--<m-flex data-type="row" class="n pL pR">

                            <m-button data-type="primary" class="" id="btnUpload${_self.name}">
                                Upload
                            </m-button>
                            <input type="file" class="none" id="upl${_self.name}" />

                        </m-flex>-->

                    </m-flex>

                </m-flex>
            
                ${getHtmlBodyList([])}

            </m-flex>

            `;
    }
    const getHtmlBodyList = function (arr) {

        let html = ``;

        for (let obj of arr)
            html += getHtmlCard(obj);
        
        return `
            <m-flex data-type="col" class="s cards selectable" id="lst${_self.name}s">

                <m-flex data-type="row" class="tableRow n pL pR mB sC" style="width: 100%;">
                    <h2>
                        Name
                    </h2>
                    <h2>
                        Email
                    </h2>
                    ${(Global.isMobile()) ? `` : `
                    <h2>
                        Skills
                    </h2>
                    <h2>
                        Personal Website
                    </h2>`}
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

            <m-flex data-type="col" class="form">

                <m-input>
                    <label for="">Name</label>
                    <p>
                        ${_self.vm.name}
                    </p>
                </m-input>

                <m-input>
                    <label for="">Title</label>
                    <p>
                        ${_self.vm.title}
                    </p>
                </m-input>

                <m-input>
                    <label for="">Company Name</label>
                    <p>
                        ${_self.vm.companyName}
                    </p>
                </m-input>

                <m-input>
                    <label for="">Phone 1</label>
                    <p>
                        ${_self.vm.phone1}
                    </p>
                </m-input>

                <m-input>
                    <label for="">Phone 2</label>
                    <p>
                        ${_self.vm.phone2}
                    </p>
                </m-input>

                <m-input>
                    <label for="">Skype ID</label>
                    <p>
                        ${_self.vm.skypeId}
                    </p>
                </m-input>

                <m-input>
                    <label for="">Email</label>
                    <p>
                        ${_self.vm.email}
                    </p>
                </m-input>

                <m-input>
                    <label for="">Company (Temp)</label>
                    <p>
                        ${_self.vm.companyTemp}
                    </p>
                </m-input>

                <m-input>
                    <label for="">Resume</label>
                    <p>
                        ${_self.vm.resume}
                    </p>
                </m-input>

                <m-input>
                    <label for="">Portfolio</label>
                    <p>
                        ${_self.vm.portfolio}
                    </p>
                </m-input>

                <m-input>
                    <label for="">Personal Website</label>
                    <p>
                        ${_self.vm.personalWebsite}
                    </p>
                </m-input>

                <m-input>
                    <label for="">Skills</label>
                    <p>
                        ${_self.vm.skills}
                    </p>
                </m-input>

                <m-input>
                    <label for="">EDC Family</label>
                    <p>
                        ${(_self.vm.isEdcFamily) ? `YES` : `NO`}
                    </p>
                </m-input>

                <m-input>
                    <label for="">Potential Staffing</label>
                    <p>
                        ${(_self.vm.isPotentialStaffing) ? `YES` : `NO`}
                    </p>
                </m-input>

            </m-flex>

            `;
    }

    const getHtmlCard = function (obj) {
        return `

            <m-card class="tableRow btnOpenModule mB" data-function="Contacts.getHtmlModuleDetail" data-args="${obj.contactId}">
                <m-flex data-type="row" class="sC tE">
                    <h2 class="tE">
                        ${obj.name}
                    </h2>
                    <h2 class="tE">
                        ${obj.email}
                    </h2>
                    ${(Global.isMobile()) ? `` : `
                    <h2 class="tE">
                        ${obj.skills}
                    </h2>
                    <h2 class="tE">
                        ${obj.personalWebsite}
                    </h2>`}
                </m-flex>
            </m-card>

            `;
    }

    const _init = (function () {
        //$(document).on(`tap`, `#btnUpload${_self.name}`, function (e) { e.stopPropagation(); e.preventDefault(); $(`#upl${_self.name}`).click(); });
        //$(document).on(`change`, `#upl${_self.name}`, function () { _upload($(this).prop(`files`)); });
        $(document).on(`tap`, `#btnShowMore${_self.name}`, function () { _self.page++; _getByPage(_self.page); });
        $(document).on(`keyup`, `#txtSearch${_self.name}`, function () { _search(); })
    })();

    return {
        getHtmlModuleDetail: getHtmlModuleDetail,
        getHtmlBody: getHtmlBody,
        getHtmlBodyList: getHtmlBodyList,
        getHtmlBodyDetail: getHtmlBodyDetail,
        getHtmlCard: getHtmlCard
    }

})();