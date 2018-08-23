(function () {
    "use strict";

    if (!Array.prototype.indexOf) {
        Array.prototype.indexOf = function (obj, start) {
            for (var i = (start || 0), j = this.length; i < j; i++) {
                if (this[i] === obj) { return i; }
            }
            return -1;
        }
    }

    function getInternetExplorerVersion() {
        var rv = -1;
        if (navigator.appName === 'Microsoft Internet Explorer') {
            var ua = navigator.userAgent;
            var re = new RegExp("MSIE ([0-9]{1,}[\.0-9]{0,})");
            if (re.exec(ua) != null)
                rv = parseFloat(RegExp.$1);
        }
        else if (navigator.appName === 'Netscape') {
            var ua = navigator.userAgent;
            var re = new RegExp("Trident/.*rv:([0-9]{1,}[\.0-9]{0,})");
            if (re.exec(ua) != null)
                rv = parseFloat(RegExp.$1);
        }
        return rv;
    }

    angular.module('AstoveApp').service('AstoveFile', ['Upload', '$location', '$http', 'AstoveService', 'AstoveData', 'AstoveDataList', 'AstoveCommon', 'AstoveError', '$q',
            function ($upload, $location, $http, $service, $data, $datalist, $global, $error, $q) {
                this.showDialog = false;
                this.propertyName = '';
                this.iframeUrl = '';
                this.baseUrl = $location.protocol() + '://' + $location.host() + (($location.port() > 0) ? ':' + $location.port() + '/' : '/');

                this.closeDialog = function () {
                    var imagemUrl = $data.entity[this.propertyName];
                    var regex = /\?\d+/g;
                    var result = regex.exec(imagemUrl);
                    if (result !== null)
                        imagemUrl = imagemUrl.replace(regex, '') + '?' + new Date().getTime();
                    else
                        imagemUrl = imagemUrl + '?' + new Date().getTime();

                    $data.entity[this.propertyName] = imagemUrl;

                    this.showDialog = false;
                };

                this.onFileSelect = function ($scope, $files, directory, type, options) {
                    var self = this;
                    if (getInternetExplorerVersion() === -1)
                        $global.loading = true;

                    var entity = $data.model;
                    if (typeof options !== 'undefined' && typeof options.isModal !== 'undefined' && options.isModal) {
                        entity = $datalist.modalModel;
                    }

                    var templateUrl = _.template("<%= baseUrl %>Common/Upload");

                    for (var i = 0; i < $files.length; i++) {
                        var file = $files[i];
                        var uploadData = {
                            url: templateUrl({ baseUrl: self.baseUrl }),
                            data: { directory: directory, type: type, propertyName: (typeof options !== 'undefined' && typeof options.propertyUrl !== 'undefined') ? options.propertyUrl : '' },
                            file: file
                        };
                        $scope.upload = $upload.upload(uploadData).progress(function (evt) {
                            console.log('percent: ' + parseInt(100.0 * evt.loaded / evt.total));
                        }).success(function (data, status, headers, config) {
                            if (typeof options !== 'undefined' && typeof options.type !== 'undefined' && options.type === 'list' && options.source !== 'image') {
                                $service.addEntityToList(null, options.entity, options.listProperty, data);
                            }
                            else if (typeof options !== 'undefined' && typeof options.type !== 'undefined' && options.type === 'array' && options.source !== 'image') {
                                $service.addEntityToChildArray(options.child, options.listProperty, options.entity, data);
                            }
                            else if (typeof options !== 'undefined' && typeof options.type !== 'undefined' && options.type === 'refresh' && options.source !== 'image') {
                                $service.onChanged($scope, options.action, options.properties);
                            }
                            else if (typeof options !== 'undefined' && typeof options.type !== 'undefined' && options.type === 'update') {
                                entity[options.propertyUrl] = data.url;
                                entity[options.propertyName] = data.fileName;
                            }
                            else if (typeof options !== 'undefined' && typeof options.type !== 'undefined' && options.type !== 'list' && options.source !== 'image') {
                                $service.addEntity($scope, options.controllerName, data, options.action, options.properties);
                            }
                            else if (typeof options !== 'undefined' && typeof options.source !== 'undefined' && options.source === 'file') {
                                if (typeof options.childName === 'undefined')
                                    entity[options.propertyName] = data.url;
                                else
                                    entity[options.childName][options.propertyName] = data.url;
                            }
                            else if (typeof options !== 'undefined' && typeof options.source !== 'undefined' && options.source === 'image') {
                                self.propertyName = options.propertyName;
                                entity[self.propertyName] = data.url;
                                self.iframeUrl = ((typeof options.actionName != 'undefined') ? options.actionName + '#/imageeditor/' : '/comum#/imageeditor/') + data.url.replace(/([\/])/g, '$') + '/' + Math.round(options.width) + '/' + Math.round(options.height);
                                $global.loading = false;
                                self.showDialog = true;
                            }

                            angular.forEach(
                                angular.element("input[type='file']"),
                                function (inputElem) {
                                    angular.element(inputElem).val(null);
                                });

                            $global.loading = false;
                        }).
                        error(function (error) {
                            $global.loading = false;
                            $error.show(errResponse, 'Não foi possível completar o upload do arquivo');
                        });
                    }
                };

                this.uploadImageModelAsync = function ($scope, $files, directory, type) {
                    var self = this;
                    var errors = [];
                    var deferred = $q.defer();

                    if ($scope.files.length == 1 && (typeof $scope.files[0].data === 'undefined' || $scope.files[0].data === null)) {
                        deferred.resolve(null);
                    }
                    else {
                        for (var i = 0; i < $files.length; i++) {
                            var file = $files[i];
                            var templateUrl = _.template("<%= baseUrl %>Common/Upload");
                            var uploadData = {
                                url: templateUrl({ baseUrl: self.baseUrl }),
                                data: { model: $upload.json({ propertyName: file.id.replace('imgCrop$', ''), directory: directory, type: type }) },
                                file: self.dataURItoBlob(file.data),
                                headers: { 'Authorization': 'Bearer ' + window.localStorage.getItem('accessToken') }
                            };
                            console.log(templateUrl({ baseUrl: self.baseUrl }));
                            $scope.upload = $upload.upload(uploadData).then(function (resp) {
                                var propertyName = file.id.replace('imgCrop$', '');
                                console.log(resp.data);
                                $data.model[resp.data.propertyName] = resp.data.url;
                                if (errors.length === 0 && i === $files.length) {
                                    deferred.resolve(resp.data);
                                }
                            }, function (resp) {
                                $error.show(resp, 'Não foi possível completar o upload do arquivo');
                                errors.push(resp.message);
                            }, function (evt) {
                                console.log('percent: ' + parseInt(100.0 * evt.loaded / evt.total));
                            });
                        }

                        if (errors.length > 0) {
                            var error = _.reduce(errors, function (memo, value) { return memo + ', ' + value; }, '');
                            deferred.reject(error);
                        }
                    }

                    return deferred.promise;
                };
                
                this.downloadPDF = function ($scope, controllerName, reportName, id) {
                    $scope.showAlert = false;
                    $scope.hasError = false;
                    $scope.success = false;
                    $global.loading = true;

                    $http.get("api/ac/" + controllerName + '/' + id, { responseType: 'arraybuffer', headers: { 'Authorization': 'Bearer ' + window.localStorage.getItem('accessToken') } })
                        .success(function (data, status, headers) {
                            $global.loading = false;
                            var octetStreamMime = 'application/octet-stream';
                            var success = false;

                            // Get the headers
                            headers = headers();

                            // Get the filename from the x-filename header or default to "download.bin"
                            var filename = headers['x-filename'] || reportName + '.pdf';

                            var contentDisposition = headers['content-disposition'];
                            if (typeof contentDisposition !== 'undefined' && contentDisposition !== null) {
                                var values = contentDisposition.split('; ');
                                if (values.length > 0) {
                                    var fileValues = values[1].split('=');
                                    if (fileValues.length === 2)
                                        filename = fileValues[1];
                                }
                            }

                            // Determine the content type from the header or default to "application/octet-stream"
                            var contentType = headers['content-type'] || octetStreamMime;

                            try {
                                // Try using msSaveBlob if supported
                                console.log("Trying saveBlob method ...");
                                var blob = new Blob([data], { type: contentType });
                                if (navigator.msSaveBlob)
                                    navigator.msSaveBlob(blob, filename);
                                else {
                                    // Try using other saveBlob implementations, if available
                                    var saveBlob = navigator.webkitSaveBlob || navigator.mozSaveBlob || navigator.saveBlob;
                                    if (saveBlob === undefined) throw "Not supported";
                                    saveBlob(blob, filename);
                                }
                                console.log("saveBlob succeeded");
                                success = true;
                            } catch (ex) {
                                console.log("saveBlob method failed with the following exception:");
                                console.log(ex);
                            }

                            if (!success) {
                                // Get the blob url creator
                                var urlCreator = window.URL || window.webkitURL || window.mozURL || window.msURL;
                                if (urlCreator) {
                                    // Try to use a download link
                                    var link = document.createElement('a');
                                    if ('download' in link) {
                                        // Try to simulate a click
                                        try {
                                            // Prepare a blob URL
                                            console.log("Trying download link method with simulated click ...");
                                            var blob = new Blob([data], { type: contentType });
                                            var url = urlCreator.createObjectURL(blob);
                                            link.setAttribute('href', url);

                                            // Set the download attribute (Supported in Chrome 14+ / Firefox 20+)
                                            link.setAttribute("download", filename);

                                            // Simulate clicking the download link
                                            var event = document.createEvent('MouseEvents');
                                            event.initMouseEvent('click', true, true, window, 1, 0, 0, 0, 0, false, false, false, false, 0, null);
                                            link.dispatchEvent(event);
                                            console.log("Download link method with simulated click succeeded");
                                            success = true;

                                        } catch (ex) {
                                            console.log("Download link method with simulated click failed with the following exception:");
                                            console.log(ex);
                                        }
                                    }

                                    if (!success) {
                                        // Fallback to window.location method
                                        try {
                                            // Prepare a blob URL
                                            // Use application/octet-stream when using window.location to force download
                                            console.log("Trying download link method with window.location ...");
                                            var blob = new Blob([data], { type: octetStreamMime });
                                            var url = urlCreator.createObjectURL(blob);
                                            window.location = url;
                                            console.log("Download link method with window.location succeeded");
                                            success = true;
                                        } catch (ex) {
                                            console.log("Download link method with window.location failed with the following exception:");
                                            console.log(ex);
                                        }
                                    }

                                }
                            }

                            if (!success) {
                                // Fallback to window.open method
                                console.log("No methods worked for saving the arraybuffer, using last resort window.open");
                                window.open("api/ac/" + controllerName + $scope.serialize(_.omit($data.model, function (value, key, object) { return _.isObject(value) })), '_blank', '');
                            }
                        })
                        .error(function (data, status) {
                            $global.loading = false;

                            console.log("Request failed with status: " + status);

                            // Optionally write the error out to scope
                            var responseData = { data: data, status: status, statusText: 'Erro ao tentar realizar o download do PDF' };
                            $error.show(responseData);
                        });
                };

                this.deleteFileEntity = function (controllerName, entity, url) {
                    $global.loading = true;
                    $http.post('/home/removefile', { url: url })
                        .success(function (data, status, headers, config) {
                            AreaClienteApi(controllerName).remove({ id: entity.id }).$promise.then(function (resp) {
                                $global.loading = false;
                            }, function (errResponse) {
                                $global.loading = false;
                                $error.show('Erro', 'OK', errResponse.data);
                            });
                        })
                        .error(function (responseData) {
                            $global.loading = false;
                            $error.show(responseData, 'Não foi possível excluir o arquivo ' + entity.url);
                        });
                };

                /**
                 * Converts data uri to Blob. Necessary for uploading.
                 * @see
                 *   http://stackoverflow.com/questions/4998908/convert-data-uri-to-file-then-append-to-formdata
                 * @param  {String} dataURI
                 * @return {Blob}
                 */
                this.dataURItoBlob = function (dataURI) {
                    // convert base64/URLEncoded data component to raw binary data held in a string
                    var byteString;
                    if (dataURI.split(',')[0].indexOf('base64') >= 0) {
                        byteString = atob(dataURI.split(',')[1]);
                    } else {
                        byteString = decodeURI(dataURI.split(',')[1]);
                    }
                    var mimeString = dataURI.split(',')[0].split(':')[1].split(';')[0];
                    var array = [];
                    for (var i = 0; i < byteString.length; i++) {
                        array.push(byteString.charCodeAt(i));
                    }
                    return new Blob([new Uint8Array(array)], { type: mimeString });
                };

                this.getImageDataURL = function (url, success, error) {
                    var data, canvas, ctx;
                    var img = new Image();
                    img.onload = function () {
                        // Create the canvas element.
                        canvas = document.createElement('canvas');
                        canvas.width = img.width;
                        canvas.height = img.height;
                        // Get '2d' context and draw the image.
                        ctx = canvas.getContext("2d");
                        ctx.drawImage(img, 0, 0);
                        // Get canvas data URL
                        try {
                            console.log(canvas);
                            data = canvas.toDataURL();
                            console.log(data);
                            success({ image: img, data: data });
                        } catch (e) {
                            error(e);
                        }
                    }
                    // Load image URL.
                    try {
                        img.src = url;
                    } catch (e) {
                        error(e);
                    }
                };

            } ]);

})();