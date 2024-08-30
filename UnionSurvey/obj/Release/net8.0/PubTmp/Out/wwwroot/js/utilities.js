'use strict';

UnionSurvey.Utilities = (function () {

    function _Init() {

        _ResetForms();
        _RemoveValidationOfFormControl();
        SortTableRow();
    }
    function _GetAvatar(id, src, initial, cls, cls2, rem) {


        if (cls.length === 0)
            cls = `avatar-ys avatar-${rem.toString().replace(".", "-").trim()}rem`;

        cls = `${cls} ${cls2}`.trim();

        if (id)
            id = `id = '${id}'`;

        var avatarHtml = "";
        if (src) {
            src = src.trim();
            avatarHtml = `<div ${id} class='${cls}'><img src='${src}' alt='${initial}'/></div>`;
        }
        else if (initial) {

            initial = initial.length == 1 ? initial.substring(0, 1) : initial.substring(0, 2);
            avatarHtml = `<div ${id} class='${cls}'><span>${initial.toUpperCase()}</span></div>`;

        } else {
            avatarHtml = `<div ${id} class='${cls}'><img src='/images/default-avatar.png' alt='${initial}'/></div>`;

        }

        return avatarHtml;
    }
    function _ImagePreview(changeEvent, errorMessageId, imagePreviewId) {

        var file = changeEvent.target.files[0];

        if (file) {

            var errorMessage = document.getElementById(errorMessageId);
            var imagePreview = document.getElementById(imagePreviewId);

            errorMessage.textContent = '';
            if (imagePreview)
                imagePreview.style.display = 'none';

            if (file.type.startsWith('image/')) {
                if (file.size <= 1 * 1024 * 1024) { // 1 MB limit
                    var reader = new FileReader();
                    reader.onload = function (e) {

                        if (imagePreview) {
                            imagePreview.src = e.target.result;
                            imagePreview.style.display = 'block';
                        }
                    }
                    reader.readAsDataURL(file);
                } else {
                    errorMessage.textContent = 'File size exceeds 2 MB.';
                }
            } else {
                errorMessage.textContent = 'Please select a valid image file.';
            }
        }
    }
    function _ScrollToEnd(id) {

        var isChat = document.getElementById(id);

        if (isChat != null)
            $(`#${id}`).animate({ scrollTop: $(`#${id}`).get(0).scrollHeight });
    }
    function _BrowserStatus() {
        if (IsChrome())
            return 'CHROME';
    }
    function IsChrome() {
        var isChrome = /Chrome/.test(navigator.userAgent) && /Google Inc/.test(navigator.vendor);
        return isChrome;
    }
    function _GetDataValueFromHtmlControl(id, control, data) {

        if (id && id.length > 0)
            control = document.getElementById(id);

        var val = $(control).data(data);
        if (val === undefined || val === null || val.length === 0)
            val = $(control).attr(`data-${data}`);

        return val;
    }
    function _SetDataValueFromHtmlControl(id, control, data, val) {
        debugger;
        if (id && id.length > 0)
            control = document.getElementById(id);

        $(control).data(data, val);
        $(control).attr(`data-${data}`, val);
    }
    function _ConvertToDateTimeLocalString(date) {
        const year = date.getFullYear();
        const month = (date.getMonth() + 1).toString().padStart(2, "0");
        const day = date.getDate().toString().padStart(2, "0");
        const hours = date.getHours().toString().padStart(2, "0");
        const minutes = date.getMinutes().toString().padStart(2, "0");

        return `${year}-${month}-${day}T${hours}:${minutes}`;
    }
    function _AjaxShowErrorMessage(result) {

        if (result.resultStatus != undefined && result.resultStatus === "Fail") {
            if (result.message == "User session ended.") {
                alert('User Session is ended.');
            } else {
                UnionSurvey.NolerNotify.Error(result.message);
            }

            return true;
        } else
            return false;
    }

    async function _DeleteConfirmAsync(recordName) {

        const deleteModal = new bootstrap.Modal('#delete-record');
        deleteModal.show()

        return new Promise((resolve, reject) => {

            document.body.addEventListener('click', response);

            function response(e) {
                let bool = false;
                if (e.target.id == 'modal-btn-descartar') bool = false;
                else if (e.target.id == 'modal-btn-aceptar') bool = true;
                else return;

                document.body.removeEventListener('click', response);
                document.body.querySelector('.modal-backdrop').remove();
                //modalElem.remove();

                deleteModal.hide();
                if (bool)
                    resolve("Success");
                else
                    reject("Error");
            }
        })
    }

    function _RemoveValidationOfFormControl() {

        $('.form-control').on('change', function () {
            if ($(this).val().trim().length > 0)
                $(this)
                    .removeClass('is-invalid')
                    .removeClass('is-valid');
        })
    }
    function _ResetForms() {
        $('.resetbtn').on('click', function () {
            // Reset the form
            _ResetControls(null, $(this));
        });
    }
    function _ResetControls(id, $this, targetFromId) {
        if (targetFromId === null || targetFromId === undefined)
            targetFromId = _GetDataValueFromHtmlControl(id, $this, 'target-form');

        if (targetFromId !== undefined && (`#${targetFromId}`) && $(`#${targetFromId}`).length > 0) {
            if ($(`#${targetFromId}`).prop('tagName').toLowerCase() === 'form')
                $(`#${targetFromId}`)[0].reset();

            // Reset hidden inputs manually if needed
            $(`#${targetFromId} input[type="hidden"]`).each(function () {
                if (this.name !== '__RequestVerificationToken') {
                    $(this).val('');
                }
            });

            var disabledControls = document.querySelectorAll(`#${targetFromId} [data-disabled]`);
            disabledControls.forEach(function (element) {
                element.disabled = false;
            });

            // Handle additional input types if necessary
            $(`#${targetFromId} input[type="text"], #${targetFromId} input[type="number"], #${targetFromId} input[type="email"], #${targetFromId} input[type="url"]`).val('');
            $(`#${targetFromId} input[type="checkbox"]`).prop('checked', false);
            $(`#${targetFromId} select`).prop('selectedIndex', 0);
        }
    }
    function _ToggleSelectAndInput($select, $input, selectOneFromSelect) {

        //Set Default values when Controls load.
        $select.prop('selectedIndex', 0);
        $input
            .hide()
            .val($select.val());

        $select.on('change', function () {

            if ($select.val() === selectOneFromSelect) {

                $select.hide();
                $input
                    .val('')
                    .show()
                    .focus();
            } else {

                var selectedText = $(`#${$select.prop('id')} option:selected`).text();


                $input
                    .hide()
                    .val(selectedText);
                $select.show();
            }
        });

        $input.on('blur', function () {
            if ($input.val().trim() !== '') {
                var val = $input.val().trim().replace(' ', '_');
                var newOption = $('<option>', {
                    value: val,
                    text: $input.val().trim()
                });
                $select.append(newOption);
                $select.val(val);
            } else {
                $select.val(1)
            }
            $input.hide();
            $select.show();
        });

        $input.on('keypress', function (e) {
            if (e.which === 13) { // Enter key

                $input.blur();
            }
        });
    }
    function SortTableRow(tableId) {

        $('.sort-checkbox').on('change', function () {

            var rows = $(`table tbody tr`).get();

            rows.sort(function (a, b) {
                return $(b).find('.sort-checkbox').prop('checked') - $(a).find('.sort-checkbox').prop('checked');
            });

            $.each(rows, function (index, row) {
                $(`table tbody`).append(row);
            });
        });
    }


    return {
        Init: _Init,
        GetAvatar: _GetAvatar,
        ScrollToEnd: _ScrollToEnd,
        BrowserStatus: _BrowserStatus,
        GetDataValueFromHtmlControl: _GetDataValueFromHtmlControl,
        SetDataValueFromHtmlControl: _SetDataValueFromHtmlControl,
        ConvertToDateTimeLocalString: _ConvertToDateTimeLocalString,
        AjaxShowErrorMessage: _AjaxShowErrorMessage,
        ImagePreview: _ImagePreview,
        DeleteConfrimAsync: async function (recordName) { await _DeleteConfirmAsync(recordName); },

        ToggleSelectAndInput: _ToggleSelectAndInput,
        ResetControl: _ResetControls
    };

})();


UnionSurvey.HTMLStorage = (function (moduleName) {

    var sessionKey = `UNIONSURVEY_${moduleName}_SESSION`;

})();

UnionSurvey.Convertion = (function () {

    function _toSafeFloorFixed(number, decimalUpTo) {

        let tensNumber = Math.pow(10, decimalUpTo);
        let flooredNumber = Math.floor(number * tensNumber) / tensNumber;
        let formattedNumber = flooredNumber.toFixed(decimalUpTo);

        // Convert to whole number if there are no decimal places
        if (Number.isInteger(flooredNumber)) {
            return parseInt(formattedNumber);
        }

        return formattedNumber;
    }

    return {
        ToSafeFloor: _toSafeFloorFixed
    }

})();