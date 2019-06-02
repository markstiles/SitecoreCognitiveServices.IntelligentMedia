jQuery.noConflict();

//setup
jQuery(document).ready(function () {
    //handles setup form
    var setupForm = ".setup-form";
    jQuery(setupForm + " .form-submit")
        .click(function (event) {
            event.preventDefault();

            var indexOptionValue = jQuery(setupForm + " input[name='indexOption']:checked").val();
            var faceValue = jQuery(setupForm + " #faceApi").val();
            var faceEndpointValue = jQuery(setupForm + " #faceApiEndpoint").val();
            var computerVisionValue = jQuery(setupForm + " #computerVisionApi").val();
            var computerVisionEndpointValue = jQuery(setupForm + " #computerVisionApiEndpoint").val();

            jQuery(".result-failure").hide();
            jQuery(".result-success").hide();
            jQuery(".progress-indicator").show();
            
            jQuery.post(
                jQuery(setupForm).attr("action"),
                {
                    indexOption: indexOptionValue,
                    faceApi: faceValue,
                    faceApiEndpoint: faceEndpointValue,
                    computerVisionApi: computerVisionValue,
                    computerVisionApiEndpoint: computerVisionEndpointValue
                }
            ).done(function (r) {
                if (r.Failed) {
                    jQuery(".result-failure .item-list").text(r.Items);
                    jQuery(".result-failure").show();
                } else {
                    jQuery(".result-success").show();
                }

                jQuery(".progress-indicator").hide();
            });
        });

    //handles reindex form
    jQuery(setupForm + " .reindex-form-submit")
        .click(function (event) {
            event.preventDefault();

            jQuery(".result-failure").hide();
            jQuery(".result-success").hide();
            jQuery(".progress-indicator").show();

            jQuery.post(
                jQuery(setupForm).attr("reindex-action"), { }
            ).done(function () {
                jQuery(".progress-indicator").hide();
            });
        });
});
