'use strict';

const Application = (function () {

    //Private ------------------------------------------------
    let _self = {
        name: `Application`
    };

    const _open = function () {

        $(`body`).prepend(Application.getHtml());
        Module.openBody(`Primary`, `Application.getHtmlBody`);
        _openSub($(`#initOpenSub`));

        //Load Lists
        Members.getById(Global.guidEmpty);
        MaterialTag.get();
        Disciplines.get();
        TimeTracker.getIsActive();
        ProjectReferenceTag.get();

    }
    const _openSub = function ($this) {
        if ($(`m-sub[data-label="${$this.html()}"]`).is(`:visible`)) return;
        $(`m-sub[data-label="${$this.html()}"]`).velocity(`stop`).velocity('slideDown', { duration: Global.velocitySettings.durationShort });
        $this.addClass(`btnCloseSub`).removeClass(`btnOpenSub`);
    }
    const _openMenu = function ($this) {

        $(`m-body[data-label="Primary"]`).removeClass(`minimized`);
        $(`m-navigation`).removeClass(`minimized`);
        $(`.btnOpenMenu`).removeClass(`btnOpenMenu`).addClass(`btnCloseMenu`);
        Global.editIcon($this, `icon-back`);

    };
    const _closeSub = function ($this) {
        if (!$(`m-sub[data-label="${$this.html()}"]`).is(`:visible`)) return;
        $(`m-sub[data-label="${$this.html()}"]`).velocity(`stop`).velocity('slideUp', { duration: Global.velocitySettings.durationShort });
        $this.addClass(`btnOpenSub`).removeClass(`btnCloseSub`);
    };
    const _closeMenu = function ($this) {

        $(`m-body[data-label="Primary"]`).addClass(`minimized`);
        $(`m-navigation`).addClass(`minimized`);
        $(`.btnCloseMenu`).removeClass(`btnCloseMenu`).addClass(`btnOpenMenu`);
        Global.editIcon($this, `icon-forward`);

    };

    const _getJackSparrow = function () {

        $.ajax({
            type: "POST",
            url: '/Methods/GetJackSparrow',
            dataType: "json",
            success: function (data) {
                console.log(data);
                Global.jack = data;
                _open();
            }
        });

    };

    //Public -------------------------------------------------
    const getHtml = function () {
        return `

            <m-navigation class="minimized">
                <m-flex data-type="col" class="n">

                    <m-flex data-type="row" class="n sC">
                        <!--<m-image class="icon sm contain tR" style="background-image: url('https://authentication.ciclops.software/Content/Images/logo_icon_white.png');"></m-image>-->
                        <m-flex data-type="col" class="w n tE">
                            <h1>Welcome back!</h1>
                            <h2 class="tE">${Global.jack.mE}</h2>
                        </m-flex>
                    </m-flex>

                    <m-flex data-type="row" class="n sC h btnOpenBody" data-label="Primary" data-function="Application.getHtmlBody" tabindex="0" role="tab" data-label="Dashboard">
                        <m-flex data-type="row" class="n c sm sQ">
                            <i class="icon-home"><svg><use xlink:href="/Content/Images/Bambino.min.svg#icon-home"></use></svg></i>
                        </m-flex>
                        <span class="tE">Dashboard<span>
                    </m-flex>

                    <!--<m-flex data-type="row" class="n sC h btnOpenBody" data-label="Primary" data-function="Contacts.getHtmlBody" tabindex="0" role="tab" data-label="Contacts">
                        <m-flex data-type="row" class="n c sm sQ">
                            <i class="icon-male-user"><svg><use xlink:href="/Content/Images/Bambino.min.svg#icon-male-user"></use></svg></i>
                        </m-flex>
                        <span class="tE">Contacts<span>
                    </m-flex>

                    <m-flex data-type="row" class="n sC h btnOpenBody" data-label="Primary" data-function="Materials.getHtmlBody" tabindex="0" role="tab" data-label="Materials">
                        <m-flex data-type="row" class="n c sm sQ">
                            <i class="icon-news"><svg><use xlink:href="/Content/Images/Bambino.min.svg#icon-news"></use></svg></i>
                        </m-flex>
                        <span class="tE">Materials<span>
                    </m-flex>-->

                    <m-flex data-type="row" class="n sC h btnOpenBody" data-label="Primary" data-function="Application.getHtmlBodyManage" tabindex="0" role="tab" data-label="Manage">
                        <m-flex data-type="row" class="n c sm sQ">
                            <i class="icon-squared-menu"><svg><use xlink:href="/Content/Images/Bambino.min.svg#icon-squared-menu"></use></svg></i>
                        </m-flex>
                        <span class="tE">Manage<span>
                    </m-flex>

                    <m-flex data-type="row" class="n sC h btnOpenBody" data-label="Primary" data-function="Settings.getHtmlBody" tabindex="0" role="tab" data-label="Settings">
                        <m-flex data-type="row" class="n c sm sQ">
                            <i class="icon-services"><svg><use xlink:href="/Content/Images/Bambino.min.svg#icon-services"></use></svg></i>
                        </m-flex>
                        <span class="tE">Settings<span>
                    </m-flex>

                </m-flex>

                <m-flex data-type="col" class="n">

                    <m-flex data-type="row" class="n sC h" id="btnSignOut" tabindex="0" role="tab" data-label="Sign Out">
                        <m-flex data-type="row" class="n c sm sQ">
                            <i class="icon-padlock"><svg><use xlink:href="/Content/Images/Bambino.min.svg#icon-padlock"></use></svg></i>
                        </m-flex>
                        <span class="tE">Sign Out<span>
                    </m-flex>

                    <m-flex data-type="row" class="n sC h btnOpenMenu">
                        <m-flex data-type="row" class="n c sm sQ">
                            <i class="icon-forward"><svg><use xlink:href="/Content/Images/Bambino.min.svg#icon-forward"></use></svg></i>
                        </m-flex>
                        <span class="tE">Collapse<span>
                    </m-flex>

                </m-flex>

            </m-navigation>

            <m-body data-label="Primary" class="minimized">
            </m-body>

            `;
    };

    const getHtmlBody = function () {
        return `

            <m-flex data-type="col" class="container">

                <m-flex data-type="row" class="n">

                    ${Project.getHtmlBody()}

                    <m-flex data-type="col" class="w">
                        Info
                    </m-flex>

                </m-flex>

            </m-flex>

            `;
    };
    const getHtmlBodyManage = function () {
        return `

            <m-flex data-type="col" class="container">

                <m-flex data-type="row" class="n s wR">

                    <m-button data-type="secondary" class="sQ btnOpenBody mR" data-label="Primary" data-function="Contacts.getHtmlBody" data-args="">
                        Contacts
                    </m-button>

                    <m-button data-type="secondary" class="sQ btnOpenBody mR" data-label="Primary" data-function="ContactCompany.getHtmlBody" data-args="">
                        Companies
                    </m-button>

                    <m-button data-type="secondary" class="sQ btnOpenBody mR" data-label="Primary" data-function="Materials.getHtmlBody" data-args="">
                        Materials
                    </m-button>

                    ${Global.jack.mIA ? `
                    <m-button data-type="secondary" class="sQ btnOpenBody mR" data-label="Primary" data-function="TimeTracker.getHtmlBody" data-args="">
                        Time Sheets
                    </m-button>` : ``}

                </m-flex>

            </m-flex>

            `;
    };

    const _init = (function () {
        $(document).on(`tap`, `.btnOpenSub`, function () { _openSub($(this)); });
        $(document).on(`tap`, `.btnCloseSub`, function () { _closeSub($(this)); });
        $(document).on(`tap`, `.btnOpenMenu`, function () { _openMenu($(this)); });
        $(document).on(`tap`, `.btnCloseMenu`, function () { _closeMenu($(this)); });

        _getJackSparrow();

    })();

    return {
        getHtml: getHtml,
        getHtmlBody: getHtmlBody,
        getHtmlBodyManage: getHtmlBodyManage
    }

})();

function onSignIn(googleUser) {

    // Useful data for your client-side scripts:
    var profile = googleUser.getBasicProfile();
    console.log("ID: " + profile.getId()); // Don't send this directly to your server!
    console.log('Full Name: ' + profile.getName());
    console.log('Given Name: ' + profile.getGivenName());
    console.log('Family Name: ' + profile.getFamilyName());
    console.log("Image URL: " + profile.getImageUrl());
    console.log("Email: " + profile.getEmail());

    // The ID token you need to pass to your backend:
    var id_token = googleUser.getAuthResponse().id_token;
    console.log("ID Token: " + id_token);
    
}