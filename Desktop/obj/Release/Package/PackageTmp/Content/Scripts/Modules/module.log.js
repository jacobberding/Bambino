'use strict';

const Log = (function () {

    //Private ----------------------------------------------
    let _self = {
        arr: [],
        vm: {},
        page: 1,
        records: 25,
        isShowMore: false,
        timeout: undefined
    }

    const _getByPage = function (page) {

        const vm = {
            page: page,
            records: _self.records,
            search: ($(`#txtSearchLog`).val()) ? $(`#txtSearchLog`).val() : ``,
            tableNames: [`Members`,`Contacts`,`Materials`]
        }

        if (page == 1) _self.arr = [];

        _self.page = page;
        Global.post(`Log_GetByTableName`, vm)
            .done(function (data) {

                _self.isShowMore = true;
                if (data.totalRecords < (vm.page * vm.records))
                    _self.isShowMore = false;

                for (let obj of data.arr)
                    _self.arr.push(obj);

                $(`m-body[data-label="Primary"] #lstLogs`).remove();
                $(`m-body[data-label="Primary"] #flxLogs`).append(Log.getHtmlBodyList(_self.arr));

            })
            .fail(function (data) {
                Validation.notification(2);
            });

    }

    const _search = function () {

        clearTimeout(_self.timeout);
        _self.timeout = setTimeout(function () {
            _getByPage(1);
        }, Global.keyUpTimeout);

    }

    //Public ----------------------------------------------
    const getByPage = function (page) {
        _getByPage(page);
    }

    const getHtmlBody = function () {
        _getByPage(1);
        return `

            <m-flex data-type="col" id="flxLogs">

                <m-flex data-type="row" class="n pL pR">

                    <h1 class="w">Activity Log</h1>

                    <m-input class="w n">
                        <input type="text" id="txtSearchLog" placeholder="Search" value="" required />
                    </m-input>

                </m-flex>
            
                ${Log.getHtmlBodyList([])}

            </m-flex>

            `;
    }
    const getHtmlBodyList = function (arr) {

        let html = ``;

        for (let obj of arr)
            html += Log.getHtmlCard(obj);

        return `
            <m-flex data-type="col" class="s cards" id="lstLogs">

                <m-flex data-type="row" class="tableRow n pL pR mB sC" style="width: 100%;">
                    <h2>
                        Activity
                    </h2>
                    <h2>
                        Member
                    </h2>
                    <h2>
                        Date
                    </h2>
                </m-flex>

                ${html}

                ${(_self.isShowMore) ? `
                <m-card class="load" id="btnShowMoreLog">
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

            <m-card class="tableRow mB">
                <m-flex data-type="row" class="sC tE">
                    <h2 class="tE">
                        ${obj.activity}
                    </h2>
                    <h2 class="tE">
                        ${obj.member.email}
                    </h2>
                    <h2 class="tE">
                        ${moment(obj.createdDate).format(`MM/DD/YYYY hh:mm a`)}
                    </h2>
                </m-flex>
            </m-card>

            `;
    }

    const _init = (function () {
        $(document).on(`tap`, `#btnShowMoreLog`, function () { _self.page++; _getByPage(_self.page); });
        $(document).on(`keyup`, `#txtSearchLog`, function () { _search(); });
    })();

    return {
        getByPage: getByPage,
        getHtmlBody: getHtmlBody,
        getHtmlBodyList: getHtmlBodyList,
        getHtmlCard: getHtmlCard
    }

})();