(function ($) {
    $.fn.CustomFileUpload = function (options) {
        var defaults = {
            URLUploadFile: null,
            SingleFileUploads: true,
            LimitMultiFileUploads: 1,
            MaxFileSize: 3145728, //3MB
            ButtonSelector: 'input[type="button"]',
            ButtonsToHideContainer: null,
            DropZoneSelector: '.fileupload-dropzone',
            DropZone: $(document),
            DoneFunction: null,
            FormData: null,
            AlternativeButtonLabel: null
        };

        var settings = $.extend(defaults, options);

        return this.each(function () {

            var self = $(this);
            var uploaderDomElement = $('<input type="file" name="files" multiple="multiple" style="opacity:0; position:absolute; top:-100px;">');
            self.append(uploaderDomElement);

            var button = self.find(settings.ButtonSelector);
            if (settings.AlternativeButtonLabel)
                button.val(settings.AlternativeButtonLabel);

            var dropZone = self.find(settings.DropZoneSelector);
            var elementsToHide;
            if (!settings.ButtonsToHideContainer) {
                elementsToHide = self.find('input[type="button"], input[type="submit"], input[type="reset"], a');
            } else {
                elementsToHide = $(settings.ButtonsToHideContainer).find(
                    'input[type="button"], input[type="submit"], input[type="reset"], a');
            }
            var dropped = false;

            self.fileupload({
                url: settings.URLUploadFile,
                singleFileUploads: settings.SingleFileUploads,
                limitMultiFileUploads: settings.LimitMultiFileUploads,
                maxFileSize: settings.MaxFileSize,
                limitConcurrentUploads: 1,
                replaceFileInput: false,
                progressInterval: 100000,
                bitrateInterval: 100000,
                disableImageHead: true,
                disableExif: true,
                disableExifThumbnail: true,
                disableExifSub: true,
                disableExifGps: true,
                disableImageMetaDataLoad: true,
                disableImageMetaDataSave: true,
                previewThumbnail: false,
                previewCanvas: false,
                disableImagePreview: true,
                disableAudioPreview: true,
                disableVideoPreview: true,
                dropZone: settings.DropZone,
                formData: settings.FormData,
                start: function (e) {
                    VisibilidadeLoading(true);
                },
                done: function (e, data) {
                    if (jQuery.type(data.result) === "string") {
                        alert(data.result);
                    }
                    else if (settings.DoneFunction) {
                        settings.DoneFunction(e, data.result);
                    }
                },
                stop: function (e) {
                    VisibilidadeLoading(false);
                }
            });

            button.click(function () {
                uploaderDomElement.trigger('click');
            });

            $(document).on({
                "dragenter": function () {
                    elementsToHide.hide();
                    dropZone.show();
                },
                "dragleave": function (e) {
                    var pageX = e.pageX || e.originalEvent.pageX;
                    if (pageX <= 0) {
                        elementsToHide.show();
                        dropZone.hide();
                    }
                },
                "dragover": function (e) {
                    e.preventDefault();
                },
                "drop": function (e) {
                    e.preventDefault();
                    if (settings.DropZone[0] != this) {
                        if (!dropped) {
                            VisibilidadeLoading(false);
                        }
                    }
                }
            });

            settings.DropZone.on({
                "drop": function (e) {
                    if (e.dataTransfer.files.length > 0) {
                        dropped = true;
                        $(".drophere", dropZone).hide();
                        $(".processing", dropZone).show();
                    }
                }
            });

            VisibilidadeLoading = function (loading) {
                if (loading) {
                    elementsToHide.hide();
                    dropZone.show();
                    $(".drophere", dropZone).hide();
                    $(".processing", dropZone).show();
                }
                else {
                    elementsToHide.show();
                    dropZone.hide();
                    $(".drophere", dropZone).show();
                    $(".processing", dropZone).hide();
                }
            }
        });
    }
})(jQuery);