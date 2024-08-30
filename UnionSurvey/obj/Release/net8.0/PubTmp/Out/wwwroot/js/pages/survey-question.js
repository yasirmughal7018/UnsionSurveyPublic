'use strict';
UnionSurvey.SurveyQuestion = (function () {

    function _init() {
        SaveQuestion();

    }

    function SaveQuestion() {

        $('#btnSaveSurveyQuestion').on('click', function () {
            var qIds = [];
            var surveyId = $('#surveyId').val() * 1;
            $('#saveMessage').hide();
            // Loop through all checked checkboxes
            $('.question-table .sort-checkbox:checked').each(function () {
                qIds.push($(this).attr('id').replace('chk_q_', '')); // Removing 'chk_q_' prefix if needed
            });

            if (qIds.length > 0) {
                let data = { "questionIds": qIds.join(','), "surveyId": surveyId };

                $.ajax({
                    type: "POST",
                    url: "/Survey/SaveSurveyQuestion",
                    data: data,
                    success: function (result) {
                        if (UnionSurvey.Utilities.AjaxShowErrorMessage(result) === false) {
                            $('#saveMessage').show();
                            // You can uncomment the following lines if needed:
                            // NolerNotify.success(result.message);
                            // $('#sqsave').text(result.message).show();
                        }
                        // $('#tblQuestions').removeClass('refresh-active');
                    }
                });
            } else {
                console.log('No questions selected.');
            }

        });

    }
    return {
        Init: _init
    };
})();