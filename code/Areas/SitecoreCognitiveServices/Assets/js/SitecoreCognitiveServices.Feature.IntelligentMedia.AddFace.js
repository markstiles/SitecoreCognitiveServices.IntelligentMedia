jQuery.noConflict();

//add face
jQuery(document).ready(function () {
    
    var addForm = ".add-face-form";
    
    //add submit
    jQuery(addForm + " .form-submit")
    .click(function (event) {
        event.preventDefault();

        var idValue = jQuery(addForm + " #id").val();
        var dbValue = jQuery(addForm + " #db").val();
        var personOptionValue = jQuery(addForm + " input[name='personOption']:checked").val();
            
        jQuery(".result-failure").hide();
        jQuery(".result-success").hide();
        jQuery(".field-title").hide();
        jQuery(".form-fields").hide();
        jQuery(".progress-indicator").show();
            
        jQuery.post(
            jQuery(addForm).attr("action"),
            {
                id: idValue,
                db: dbValue,
                personid: personOptionValue
            }
        ).done(function (r) {
            if (r.Failed) {
                jQuery(".result-failure").show();
            } else {
                jQuery(".result-success").show();
            }

            jQuery(".progress-indicator").hide();
        });
    });
});
