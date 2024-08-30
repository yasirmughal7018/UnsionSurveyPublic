'use strict';
UnionSurvey.UserAdmin = (function () {

    let _tid = 0;
    function _init() {

        const adjustmentModal = document.getElementById('adjustmentModalBackdrop')
        if (adjustmentModal) {
            adjustmentModal.addEventListener('show.bs.modal', event => {
                // Button that triggered the modal
                const button = event.relatedTarget
                // Extract info from data-bs-* attributes
                const userName = button.getAttribute('data-bs-whatever')
                const usercode = button.getAttribute('data-bs-usercode')
                const status = button.getAttribute('data-bs-status')
                const tid = button.getAttribute('data-bs-tid')
                _tid = tid;
                // If necessary, you could initiate an Ajax request here
                // and then do the updating in a callback.

                // Update the modal's content.
                const modalTitle = adjustmentModal.querySelector('.modal-title')
                const modalBodyInput = adjustmentModal.querySelector('.modal-body input')

                modalTitle.textContent = `Adjustment for ${userName}`
                $('#username').val(userName);

                if (status === "INPROGRESS") {
                    $('#activateUser').show();
                }
                //modalBodyInput.value = recipient
            })

            adjustmentModal.addEventListener('hide.bs.modal', event => {
                // Update the modal's content.
                const modalTitle = adjustmentModal.querySelector('.modal-title')

                modalTitle.textContent = '';
                $('#activateUser').hide();
                $('#username').val('');
                $('#adjustmentamount').val(0);
                $('#error').hide();
                //modalBodyInput.value = recipient
            })
            SaveAdjustment();
            ActiveUser();
        }
    }

    function SaveAdjustment() {

        $('#saveAdjustment').on('click', function () {
            let amount = $('#adjustmentamount').val() * 1;
            let username = $('#username').val();

            if (amount == 0) {
                $('#error').show();
            } else {
                $('#error').hide();

                // $('#depositfund').addClass('refresh-active');

                let tnxAmount = { "username": username, "amount": amount };

                $.ajax({
                    type: "POST",
                    url: "/User/AdjustmentAmount",
                    data: tnxAmount,
                    headers: { "RequestVerificationToken": $('input:hidden[name="__RequestVerificationToken"]').val() },
                    success: function (result) {

                        if (UnionSurvey.Utilities.AjaxShowErrorMessage(result) === false) {

                            var divBalance = `#balance_${username}`;
                            var existingAmount = $(divBalance).text().trim() * 1 + amount;
                            $(divBalance).text(existingAmount);

                            $('#adjustmentModalBackdrop').modal('hide');

                        }
                        // $('#depositfund').removeClass('refresh-active');
                    }
                });
            }
        });
    }

    function ActiveUser() {

        $('#activateUser').on('click', function () {
            let username = $('#username').val();

            let tnxAmount = { "username": username, "tid": _tid };
            debugger;

            $.ajax({
                type: "POST",
                url: "/User/ActivateUser",
                data: tnxAmount,
                headers: { "RequestVerificationToken": $('input:hidden[name="__RequestVerificationToken"]').val() },
                success: function (result) {

                    debugger;
                    if (UnionSurvey.Utilities.AjaxShowErrorMessage(result) === false) {

                        UnionSurvey.Utilities.SetDataValueFromHtmlControl(`btn_${username}`, null, 'bs-status', 'MANUAL FAILED');
                        var spanActive = `#spanactive_${username}`;
                        $(spanActive).text('MANUAL FAILED');

                        $('#adjustmentModalBackdrop').modal('hide');

                    }
                    // $('#depositfund').removeClass('refresh-active');
                }
            });
        });
    }


    return {
        Init: _init
    };
})();