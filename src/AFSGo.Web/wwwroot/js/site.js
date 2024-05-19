$(document).ready(() => {
    $('#translationForm').submit(function (event) {
        event.preventDefault();

        const thisForm = $(this);

        const fields = thisForm.find('.form-control');
        console.log(fields);
        fields.removeClass('is-invalid');

        let anyError = false;
        fields.each(function () {
            if ($(this).val().trim() === '') {
                anyError = true;
                $(this).addClass('is-invalid');
            }
        });
        if (anyError) {
            return;
        }

        const packedValue = thisForm.find('#translator').val().split("/");
        if (packedValue.length !== 2) {
            $('#translationForm #translator').addClass('is-invalid');
            console.error('Sanity check. This could not happen. Probably bug in select option value. This must have format \"translator/language\"');
            return;
        }
        const [translator, language] = packedValue;
        const data = {
            translator, language, inputText: thisForm.find(' #inputText').val()
        };
        $.post({
            url: '/Translation/Translate',
            data,
            success: response => $('#translated-output').text(response.translatedText),
            error: (xhr, status, error) => console.error(error)
        });
    });
})