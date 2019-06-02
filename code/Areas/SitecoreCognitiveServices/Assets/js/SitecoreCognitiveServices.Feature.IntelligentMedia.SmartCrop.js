jQuery.noConflict();

//smart crop
jQuery(document).ready(function () {

    jQuery(".save-form-submit").hide();
    jQuery(".crop-form-back").hide();

    var smartCropForm = ".smart-crop-form";

    //aspect ratio
    jQuery(smartCropForm + " #height, " + smartCropForm + " #width")
    .change(function() {
        var preserveAspectValue = jQuery(smartCropForm + " #preserveAspectRatio").is(":checked");
        if (!preserveAspectValue)
            return;

        var isHeight = jQuery(this).attr("id") === "height";
        var otherInput = isHeight ? "width" : "height";
        var aspectRatio = parseFloat(jQuery(smartCropForm + " #aspectRatio").val());
        var thisValue = parseFloat(jQuery(this).val());
        var newValue = isHeight ? thisValue * aspectRatio : thisValue / aspectRatio;
        jQuery(smartCropForm + " #" + otherInput).val(parseInt(newValue));
    });

    //reset submit
    jQuery(smartCropForm + " .reset-form-submit")
        .click(function(event) {
            event.preventDefault();

            var originalHeightValue = jQuery(smartCropForm + " #height").data("original");
            jQuery(smartCropForm + " #height").val(originalHeightValue);
            var originalWidthValue = jQuery(smartCropForm + " #width").data("original");
            jQuery(smartCropForm + " #width").val(originalWidthValue);
        });

    //crop submit
    jQuery(smartCropForm + " .crop-form-submit")
    .click(function (event) {
        event.preventDefault();

        var idValue = jQuery(smartCropForm + " #id").val();
        var dbValue = jQuery(smartCropForm + " #db").val();
        var heightValue = jQuery(smartCropForm + " #height").val();
        var widthValue = jQuery(smartCropForm + " #width").val();
        
        jQuery(".crop-failure").hide();
        jQuery(".save-failure").hide();
        jQuery(".result-success").hide();
        jQuery(".form-fields").hide();
        jQuery(".progress-indicator").show();
            
        jQuery.post(
            jQuery(smartCropForm).attr("crop-action"),
            {
                id: idValue,
                db: dbValue,
                height: heightValue,
                width: widthValue
            }
        ).done(function (r) {
            if (r.Failed) {
                jQuery(".crop-failure").show();
                for (var i = 0; i < r.Messages.length; i++) {
                    var message = r.Messages[i];
                    jQuery(".crop-failure .error-messages").append("<li>" + message + "</li>");
                }
            } else {
                jQuery(".crop-preview").html("<img src='/" + r.FileName + "'/>");
                jQuery("#fileName").val(r.FileName);
                jQuery(".crop-preview").show();
                jQuery(".save-form-submit").show();
                jQuery(".crop-form-back").show();
                jQuery(".reset-form-submit").hide();
            }

            jQuery(".crop-form-submit").hide();
            jQuery(".progress-indicator").hide();
        });
    });

    //crop back
    jQuery(smartCropForm + " .crop-form-back")
    .click(function (event) {
        event.preventDefault();

        jQuery(".crop-failure").hide();
        jQuery(".save-failure").hide();
        jQuery(".result-success").hide();
        jQuery(".form-fields").show();
        jQuery(".progress-indicator").hide();
        jQuery(".crop-preview").hide();
        jQuery(".save-form-submit").hide();
        jQuery(".crop-form-submit").show();
        jQuery(".reset-form-submit").show();
        jQuery(this).hide();
    });

    //save submit
    jQuery(smartCropForm + " .save-form-submit")
    .click(function (event) {
        event.preventDefault();

        var idValue = jQuery(smartCropForm + " #id").val();
        var dbValue = jQuery(smartCropForm + " #db").val();
        var fileNameValue = jQuery(smartCropForm + " #fileName").val();
        var widthValue = jQuery(smartCropForm + " #width").val();
        var heightValue = jQuery(smartCropForm + " #height").val();

        jQuery(".crop-failure").hide();
        jQuery(".save-failure").hide();
        jQuery(".result-success").hide();
        jQuery(".crop-preview").hide();
        jQuery(".progress-indicator").show();

        jQuery.post(
            jQuery(smartCropForm).attr("save-action"),
            {
                id: idValue,
                db: dbValue,
                fileName: fileNameValue,
                width: widthValue,
                height: heightValue
            }
        ).done(function (r) {
            if (r.Failed) {
                jQuery(".save-failure").show();
            } else {
                jQuery(".result-success").show();
                jQuery(".form-fields").show();
                jQuery(".crop-form-submit").show();
                jQuery(".crop-form-back").hide();
                jQuery(".reset-form-submit").click();
                jQuery(".reset-form-submit").show();
            }

            jQuery(".save-form-submit").hide();
            jQuery(".progress-indicator").hide();
        });
    });
});
