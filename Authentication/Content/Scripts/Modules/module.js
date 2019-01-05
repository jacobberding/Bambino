'use strict';

const Module = (function () {
    
    //Private ----------------------------------------------------------
    let _history = [];

    const _getIsChildInParent = function ($child, parentTag) {
        return ($child.parents(parentTag).length) ? true : false;
    }

    const _openScrim = function () {
        if ($(`m-studio`).length == 0) $(`m-scrim`).velocity('stop').css(`z-index`, 1000).velocity({ opacity: .5 }, Global.velocitySettings.options);
    }
    const _closeScrim = function () {
        if ($(`m-studio`).length == 0) $("m-scrim").velocity('stop').velocity({ opacity: 0 }, { duration: Global.velocitySettings.durationShort, easing: Global.velocitySettings.easing, complete: function () { $(this).css(`z-index`, -1000); } });
    }
    const _openModuleComplete = function (f = ``, args = ``, c = ``) {

        if ($(`m-studio`).length == 0)
            _history.push({ f: f, args: args, c: c }); 

        $(`m-module`).attr(`class`, c).html(Global.getFunctionByName(f, args));
        $(`m-module .tab:first`).addClass(`active`);
        $(`m-module`).velocity(`transition.slideLeftIn`, Global.velocitySettings.options);
        
    }
    const _closeModuleComplete = function () {
        
        if (_history.length == 0) {
            $(`m-module, m-scrim`).remove();
            return;
        }
        
        const obj = _history[_history.length - 1];

        $(`m-module`).attr(`class`, obj.c).html(Global.getFunctionByName(obj.f, obj.args))
        $(`m-module .btnOpenBody:first`).addClass(`active`);
        $(`m-module`).velocity(`transition.slideLeftIn`, Global.velocitySettings.options);

    }

    //Public ----------------------------------------------------------
    
    const getHtmlConfirmation = function (action, id, dataId = ``) {
        return `

            <m-header data-label="Confirmation Header">
                <m-flex data-type="row" class="n">
                    <m-flex data-type="row" class="n c tab h">
                        <span>Confirmation</span>
                    </m-flex>
                </m-flex>
                <m-flex data-type="row" class="n c sQ h btnCloseModule">
                    <i class="icon-delete"><svg><use xlink:href="/Content/Images/Bambino.min.svg#icon-delete"></use></svg></i>
                </m-flex>
            </m-header>

            <m-body data-label="Confirmation Body">

                <m-flex data-type="col">

                    <h1 style="font-weight: 800;margin-bottom: .5em;">Confirmation</h1>
                    <p>Are you sure you want to DELETE this ${action}?</p>
                
                </m-flex>

                <m-flex data-type="row" class="footer">
                    <m-button data-type="secondary" class="btnCloseModule">
                        No
                    </m-button>

                    <m-button data-type="primary" id="${id}" data-id="${dataId}">
                        Yes
                    </m-button>
                </m-flex>

            </m-body>

            `;
    }
    const getHtmlBodyLoading = function () {
        return `

            <m-flex data-type="col" class="c container">

                <h2 class="mB">Loading</h2>
                <h1 class="loading">Loading</h1>
            
            </m-flex>

            `;
    }

    const openModule = function (f = ``, args = ``, c = ``) {
        
        if ($(`m-module`).length == 0) {
            _history = [];
            $(`body`).prepend(`<m-module></m-module><m-scrim></m-scrim>`);
            _openScrim();
            _openModuleComplete(f, args, c);
        } else {
            $(`m-module`).velocity(`stop`).velocity(`transition.slideLeftOut`, {
                duration: Global.velocitySettings.durationShort, 
                easing: Global.velocitySettings.easing,
                display: `flex`,
                complete: function () { _openModuleComplete(f, args, c); }
            });
        }

    }
    const openBody = function (label, f = ``, args = ``) {

        const i = (args != ``) ? `[data-args="${args}"]` : ``;

        if (_getIsChildInParent($(`.btnOpenBody[data-function="${f}"]`), `m-navigation`))
            $(`m-navigation .btnOpenBody`).removeClass(`active`);

        $(`.btnOpenBody[data-function="${f}"]${i}`).parent().find(`.btnOpenBody`).removeClass(`active`);
        $(`.btnOpenBody[data-function="${f}"]`).removeClass(`active`);
        $(`.btnOpenBody[data-function="${f}"]${i}`).addClass(`active`);

        setTimeout(function () { //NEED the timeout for mobile apparently moving the thing you are in causes it to simultaneously stop that animation
            $(`m-body[data-label="${label}"]`).velocity(`stop`).velocity('transition.slideDownOut', {
                duration: Global.velocitySettings.durationShort, 
                easing: Global.velocitySettings.easing,
                display: `block`,
                complete: async function () {
                    const html = await Global.getFunctionByName(f, args);
                    $(`m-body[data-label="${label}"]`).html(html)
                        //.append((label == `Primary` && f != `Application.getHtmlBodySuccess`) ? Global.getHtmlFooter() : ``)
                        .velocity('transition.slideUpIn', Global.velocitySettings.options);
                    $(`html, body`).animate({
                      scrollTop: 0
                    }, 200);
                }
            });
        }, 100);
    }
    const openSubMenu = function (id) {

        const el = $(`m-card[data-id="${id}"]`).find(`m-submenu`);
        const transition = (el.is(':visible')) ? `transition.slideDownOut` : `transition.slideUpIn`;

        el.velocity(`stop`).velocity(transition, Global.velocitySettings.options);

    }

    const editCard = function ($this, f = ``, args = ``) {
        console.log(`btnEditCard`);
        const $el = ($this.parentsUntil(`m-card`).parent().length == 0) ? $this.parent() : $this.parentsUntil(`m-card`).parent();
        console.log($el);
        $el.velocity(`stop`).velocity('transition.slideRightOut', {
            duration: Global.velocitySettings.durationShort, 
            easing: Global.velocitySettings.easing,
            display: `flex`,
            complete: function () {
                $el.html(Global.getFunctionByName(f, args)).velocity('transition.slideLeftIn', Global.velocitySettings.optionsFlex);
                $(`m-tooltip`).remove();
            }
        });
    }
    const replaceCard = function (label, f, args) {
        $(`m-card[data-label="${label}"]`).velocity('transition.slideRightOut', {
            duration: Global.velocitySettings.durationShort, 
            easing: Global.velocitySettings.easing,
            complete: function () {
                const $el = $(Global.getFunctionByName(f, {}));
                $(`m-card[data-label="${label}"]`).replaceWith($el);
                $el.velocity('transition.slideLeftIn', Global.velocitySettings.options);
            }
        });
    }

    const closeModule = function (deleteCount = 1) {

        _history.splice(_history.length - deleteCount, deleteCount); 
        
        if (_history.length == 0) _closeScrim();

        $(`m-module`).velocity(`stop`).attr(`class`, ``).velocity(`transition.slideLeftOut`, {
            duration: Global.velocitySettings.durationShort, 
            easing: Global.velocitySettings.easing,
            display: `none`,
            complete: function () { _closeModuleComplete(); }
        });

    }
    const closeModuleAll = function () {
        
        _history = []; 
        
        if (_history.length == 0) _closeScrim();

        $(`m-module`).velocity(`stop`).velocity(`transition.slideLeftOut`, {
            duration: Global.velocitySettings.durationShort, 
            easing: Global.velocitySettings.easing,
            display: `none`,
            complete: function () { $(`m-module, m-scrim`).remove(); }
        });

    }
    
    const _init = (function () {
        $(document).on(`tap`, `.btnCloseModule`, function () { Module.closeModule($(this).attr(`data-deleteCount`)); });
        $(document).on(`tap`, `.btnOpenModule`, function () { Module.openModule($(this).attr(`data-function`), $(this).attr(`data-args`), $(this).attr(`data-class`)); });
        $(document).on(`tap`, `.btnOpenBody`, function (e) { e.preventDefault(); e.stopImmediatePropagation(); Module.openBody($(this).attr(`data-label`), $(this).attr(`data-function`), $(this).attr(`data-args`)); });
        $(document).on(`tap`, `.btnEditCard`, function (e) { e.preventDefault(); e.stopImmediatePropagation(); Module.editCard($(this), $(this).attr(`data-function`), $(this).attr(`data-args`)); });
        $(document).on(`tap`, `.btnReplaceCard`, function (e) { e.preventDefault(); e.stopImmediatePropagation(); Module.replaceCard($(this).attr(`data-label`), $(this).attr(`data-function`), $(this).attr(`data-args`)); });
        $(document).on(`tap`, `.btnOpenSubMenu`, function () { Module.openSubMenu($(this).attr(`data-id`)); });
    })();

    return {
        getHtmlConfirmation: getHtmlConfirmation,
        getHtmlBodyLoading: getHtmlBodyLoading,
        openModule: openModule,
        openBody: openBody,
        openSubMenu: openSubMenu,
        editCard: editCard,
        replaceCard: replaceCard,
        closeModule: closeModule,
        closeModuleAll: closeModuleAll
    }

})();