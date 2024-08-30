'use strict';
UnionSurvey.Survey = (function () {

    function _init() {
        Delete();
    }

    function Delete() {
        $('.deleterecord').on('click', function () {

            const id = UnionSurvey.Utilities.GetDataValueFromHtmlControl(null, $(this), 'surveyId');

            //$('#surveys').addClass('refresh-active');

            let survey = { "surveyId": id };

            $.ajax({
                type: "POST",
                url: "/Survey/DeleteSurvey",
                data: survey,
                headers: { "RequestVerificationToken": $('input:hidden[name="__RequestVerificationToken"]').val() },
                success: function (result) {

                    if (UnionSurvey.Utilities.AjaxShowErrorMessage(result) === false) {
                        //NolerNotify.success(result.message);
                        var divId = "#survey_" + id;
                        $(divId).remove();
                    }
                    $('#surveys').removeClass('refresh-active');
                }
            });
        })


    }

    return {
        Init: _init
    };
})();