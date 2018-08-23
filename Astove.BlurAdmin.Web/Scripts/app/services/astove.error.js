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

    angular.module('AstoveApp').service('AstoveError', ['$uibModal',
        function ($modal) {
            this.hasErrors = false;
            this.title = '';
            this.buttonText = '';
            this.errorItems = [];

            this.show = function (response, title, alert) {
                var templateUrl = '/Error/Error500';
                var message = 'Ocorreu um problema ao tentar executar a sua solicitação';
                var errors = [];
                var exception = _.noop;

                if (typeof response === 'undefined') {
                    templateUrl = '/Error/Alert';
                    message = title;
                }
                else if (response.status === 200) {
                    templateUrl = '/Error/Success200';
                    message = response.statusText;
                }
                else if (response.status === 400) {
                    templateUrl = '/Error/Error400';
                    message = 'Requisição inválida';
                    var keys = _.keys(response.data.modelState);
                    _.forEach(keys, function (key) {
                        _.forEach(response.data.modelState[key], function (error) {
                            errors.push(error);
                        });
                    });
                }
                else if (response.status === 401) {
                    templateUrl = '/Error/Error401';
                    message = 'Este recurso não está autorizado para o seu usuário';
                }
                else if (response.status === 404) {
                    templateUrl = '/Error/Error404';
                    message = 'Registro não encontrado';
                }
                else if (response.status === 408) {
                    templateUrl = '/Error/Error408';
                    message = response.data.statusText;
                }
                else if (response.status === 500) {
                    templateUrl = '/Error/Error500';
                    message = response.data.exceptionMessage;
                    exception = { exceptionMessage: response.data.exceptionMessage, stackTrace: response.data.stackTrace }
                }
                else if (response.status === 502) {
                    templateUrl = '/Error/Error500';
                    message = 'Verifique a sua conexão com a internet';
                }

                if (typeof alert === 'undefined') {
                    var modalInstance = $modal.open({
                        templateUrl: templateUrl,
                        controller: 'ErrorModalCtrl',
                        resolve: {
                            astoveParams: function () { return { message: message, title: title, errors: errors, exception: exception }; }
                        }
                    });
                }
                else {
                    if (errors.length == 0)
                        window.alert(message);
                    else
                        window.alert(message + ':\n' + errors.join('\n'));
                }
            };

            this.showAlert = function (message, alert) {
                var templateUrl = '/Error/Alert';
              
                if (typeof alert === 'undefined') {
                    var modalInstance = $modal.open({
                        templateUrl: templateUrl,
                        controller: 'ErrorModalCtrl',
                        resolve: {
                            astoveParams: function () { return { message: message }; }
                        }
                    });
                }
                else {
                    window.alert(message);
                }
            };

            this.getMessage = function (response, text) {
                var message = 'Ocorreu um problema ao tentar executar a sua solicitação';
                var errors = [];
                var exception = _.noop;

                if (typeof response === 'undefined') {
                    message = text;
                }
                else if (response.status === 200) {
                    message = text;
                }
                else if (response.status === 400) {
                    var keys = _.keys(response.data.modelState);
                    message = 'Requisição inválida';
                    _.forEach(keys, function (key) {
                        _.forEach(response.data.modelState[key], function (error) {
                            errors.push(error);
                        });
                    });                    
                }
                else if (response.status === 401) {
                    message = 'Este recurso não está autorizado para o seu usuário';
                }
                else if (response.status === 404) {
                    message = 'Registro não encontrado';
                }
                else if (response.status === 408) {
                    message = response.data.statusText;
                }
                else if (response.status === 500) {
                    message = response.data.exceptionMessage;
                }
                else if (response.status === 502) {
                    message = 'Verifique a sua conexão com a internet';
                }

                if (errors.length == 0)
                    return message;
                else
                    return message + ':\n' + errors.join('\n');
            };

            this.showError = function (titleText, buttonLabel, errors) {
                if (typeof errors !== 'undefined' && errors.length > 0) {
                    this.title = titleText;
                    this.buttonText = buttonLabel;
                    this.errorItems = errors;
                    this.hasErrors = true;
                }
            };

            this.hideError = function () {
                this.hasErrors = false;
                this.title = '';
                this.buttonText = '';
                this.errorItems = [];
            };
        }]);

})();