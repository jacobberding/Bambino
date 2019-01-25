'use strict';

const Log = (function () {

    //Private ----------------------------------------------
    let _timeout;
    const _records = 25;
    let _page = 1;
    let _isShowMore = false;
    let _arr = [];

    const _getByPage = function (page) {

        const vm = {
            page: page,
            records: _records,
            search: ($(`#txtSearchLog`).val()) ? $(`#txtSearchLog`).val() : ``,
            tableNames: [`Members`,`Contacts`,`Materials`]
        }

        if (page == 1) _arr = [];

        _page = page;
        Global.post(`Log_GetByTableName`, vm)
            .done(function (data) {

                _isShowMore = true;
                if (data.totalRecords < (vm.page * vm.records))
                    _isShowMore = false;

                for (let obj of data.arr)
                    _arr.push(obj);

                $(`m-body[data-label="Primary"] #lstLogs`).remove();
                $(`m-body[data-label="Primary"] #flxLogs`).append(Log.getHtmlBodyList(_arr));

            })
            .fail(function (data) {
                Validation.notification(2);
            });

    }

    const _search = function () {

        clearTimeout(_timeout);
        _timeout = setTimeout(function () {
            _getByPage(1);
        }, Global.keyUpTimeout);

    }

    //Public ----------------------------------------------
    let arr = [];
    let vm = {};
    const init = function () {
        $(document).on(`tap`, `#btnShowMoreLog`, function () { _page++; _getByPage(_page); });
        $(document).on(`keyup`, `#txtSearchLog`, function () { _search(); });
        _getByPage(1);
    }

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

                ${(_isShowMore) ? `
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

    return {
        arr: arr,
        vm: vm,
        init: init,
        getByPage: getByPage,
        getHtmlBody: getHtmlBody,
        getHtmlBodyList: getHtmlBodyList,
        getHtmlCard: getHtmlCard
    }

})();