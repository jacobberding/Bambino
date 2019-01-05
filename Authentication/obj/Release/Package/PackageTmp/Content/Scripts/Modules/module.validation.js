'use strict';

const Validation = (function () {
    
	let _passwords = [];
	let _errors = 0;
	let _btnHtml = ``;
	let _btn;
	let _timeout;
    
	//Private -----------------------------------------------
	const _getIsValidEmail = function (email) {
		var re = /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
		return re.test(email);
	}
	const _getIsValidElement = function ($this) {
    
		if ($this.attr("type") == "password")
			_passwords.push({ val: $this.val(), el: $this });
    
		if ((($this.attr("type") == "tel" || $this.attr("type") == "text" || $this.attr("type") == "number" || $this.attr("type") == "password" || $this.attr("type") == "date" || $this.is("textarea")) && $.trim($this.val()) == "")
            || ($this.attr("type") == "radio" && !$(`input[name='${$this.attr("name")}']:checked`).val()) //Radio Buttons
            || ($this.is("select") && $this.val() == 0) //Dropdown
            || ($this.attr("type") == "email" && !_getIsValidEmail($this.val()))) //Email
			Validation.addError($this);
        
	}

	//Public -----------------------------------------------
	const init = function () {
		$(document).on(`keyup change`, `.error`, function () {
			$(this).removeClass("error");
			$(this).parent().find('m-error').remove();
		});
		$(document).on(`tap`, `.btnCloseNotification`, function () { $(`m-notification`).remove(); });
	}

	const addError = function ($this) {
    
		_errors++;
    
		$this.addClass(`error`);
		$this.parent().find(`m-error`).remove();
		//$this.parent().append(`<m-error>Please fill in the ${$this.parent().find(`label`).html()} field.</m-error>`);
		$this.parent().append(`<m-error>*Required</m-error>`);

	}
	const addErrorGeneric = function (ex) {
        
        const t = (ex.includes(`Passwords`)) ? `Missing Requirement` : `Error`;

		_errors++;
        console.log(ex);
		Validation.notification(1, t, ex, `error`);
		//$this.find('m-error[data-type="generic"]').remove();
		//$this.append(`<m-error data-type="generic">${ex}</m-error>`);

	}
    
	const getIsValidForm = function ($parent) {
        
		let error = `An error has occured.`;

		_btn = $parent.find(`m-button[data-type="primary"]`);
		_passwords = [];
		_btnHtml = (Global.ajaxInProgress) ? _btnHtml : _btn.html();
		$(`m-error`).remove();
		$(`.error`).removeClass(`error`);

		if (Global.ajaxInProgress) Validation.addErrorGeneric(`Syncing data . . .`);
		_btn.addClass("disabled").html(`<i class="icon-restart"><svg style="width: 20px;height: 20px;margin-top: 8px;fill: #FFF;"><use xlink:href="/Content/Images/Bambino.min.svg#icon-restart"></use></svg></i>`);
    
		$.each($parent.find("input,select,textarea"), function () { if ($(this).attr("required")) _getIsValidElement($(this)); });
    
		if (_passwords.length == 2 && _passwords[0].val != _passwords[1].val) {
			_errors++;
			error = `Passwords do not match, please try again.`;
		    $parent.find(`[type="password"]`).addClass(`error`);
		    $parent.find(`[type="password"]`).parent().find(`m-error`).remove();
		    $parent.find(`[type="password"]`).parent().append(`<m-error>*Required</m-error>`);
		}

		if (_passwords.length >= 1)
			for (let _password of _passwords)
                if (_password.val.length < 8 || !/[0-9]/.test(_password.val)) { // || !/[!@#$%&*]/.test(_password.val)
				    _errors++;
				    error = `Passwords must be at least 8 characters in length and must contain at least one number.`; //at least one special character and
		            $parent.find(`[type="password"]`).addClass(`error`);
		            $parent.find(`[type="password"]`).parent().find(`m-error`).remove();
		            $parent.find(`[type="password"]`).parent().append(`<m-error>*Required</m-error>`);
			    }
        
		if (_errors > 0)
			throw error;
    
	}

	const done = function () {
		_btn.removeClass(`disabled`).html(_btnHtml);
		_errors = 0;
	}
	const fail = function (data) {
		console.log(data);
		console.log(Global.ajaxInProgress);

		setTimeout(function () { if (!Global.ajaxInProgress) _btn.removeClass(`disabled`).html(_btnHtml); }, 100);
        
        if (typeof data.responseJSON !== "undefined")
            Validation.addErrorGeneric(data.responseJSON.Message);
        else
		    Validation.addErrorGeneric(data);

		_errors = 0;

	}
	const notification = function (type = 1, t = `Success`, m = `Request successfully processed.`, c = `success`) {
        
		if (type == 2) { t = `Error`; m = `An error has occurred.`; c = `error`; };

		$("m-notification").remove();
		$(`body`).append(`<m-notification class="${c} d1">
                <h1>
                    <span>${t}</span>
                    <i class="icon-delete btnCloseNotification"><svg style="width: 20px;height: 20px;"><use xlink:href="/Content/Images/Bambino.min.svg#icon-delete"></use></svg></i>
                </h1>
                <p>${m}</p>
            </m-notification>`);
        
		clearTimeout(_timeout);
		_timeout = setTimeout(function () {
			$("m-notification").remove();
		}, 10000);

	}

	return {
		init: init,
		getIsValidForm: getIsValidForm,
		addError: addError,
		addErrorGeneric: addErrorGeneric,
		done: done,
		fail: fail,
		notification: notification
	}

})();

Validation.init();