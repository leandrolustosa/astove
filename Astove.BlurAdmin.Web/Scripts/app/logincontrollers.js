(function () {
    "use strict";

    angular.module('loginApp').controller('loginCtrl', ['$scope', '$http', '$window', function ($scope, $http, $window) {
        $scope.hasErrors = false;

        $scope.model = {
            userName: '',
            password: '',
            rememberMe: false
        };

        // function to submit the form after all validation has occurred
        $scope.submitForm = function (isValid) {
            $scope.hasErrors = false;

            if (isValid) {
                console.log($scope.antiForgeryToken);
                $http({
                    method: 'POST',
                    url: '/Account/Login',
                    data: $scope.model,
                    headers: {
                        'RequestVerificationToken': $scope.antiForgeryToken
                    }
                }).success(function (data, status, headers, config) {
                    $scope.message = '';
                    $scope.hasErrors = !data.success;
                    if (data.success == false) {
                        var str = '';
                        for (var error in data.errors) {
                            str += data.errors[error] + '\n';
                        }
                        $scope.message = str;
                        console.log(str);
                        console.log($scope.hasErrors);
                    }
                    else {
                        console.log('Saved Successfully');
                        $http({
                            method: 'POST',
                            url: '/Token',
                            headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
                            transformRequest: function (obj) {
                                var str = [];
                                for (var p in obj)
                                    str.push(encodeURIComponent(p) + "=" + encodeURIComponent(obj[p]));
                                return str.join("&");
                            },
                            data: { grant_type: 'password', username: $scope.model.userName, password: $scope.model.password }
                        }).success(function (data) {
                            console.log(data.access_token);
                            window.localStorage.setItem("accessToken", data.access_token);
                            $window.location.href = "/";
                        });                        
                    }
                }).error(function (data, status, headers, config) {
                    $scope.message = 'Unexpected Error';
                    console.log('Unexpected Error');
                });
            }
        };
    }]);

    angular.module('loginApp').controller('forgotPasswordCtrl', ['$scope', '$http', '$window', function ($scope, $http, $window) {
        $scope.success = false;
        $scope.hasErrors = false;

        $scope.model = {
            UserName: ''
        };

        // function to submit the form after all validation has occurred
        $scope.submitForm = function (isValid) {
            $scope.success = false;
            $scope.hasErrors = false;

            if (isValid) {
                $http({
                    method: 'POST',
                    url: '/Account/ForgotPassword',
                    data: $scope.model,
                    headers: {
                        'RequestVerificationToken': $scope.antiForgeryToken
                    }
                }).success(function (data, status, headers, config) {
                    $scope.message = '';
                    $scope.success = data.success;
                    $scope.hasErrors = !data.success;
                    if (data.success == false) {
                        var str = '';
                        for (var error in data.errors) {
                            str += data.errors[error] + '\n';
                        }
                        $scope.message = str;
                    }
                    else {
                        $scope.message = 'Uma nova senha foi enviada para o seu e-mail';
                    }
                }).error(function (data, status, headers, config) {
                    $scope.hasErrors = true;
                    $scope.message = 'Erro inesperado, tente novamente mais tarde!';
                });
            }
        };
    }]);

    angular.module('loginApp').controller('profileCtrl', ['$scope', '$http', '$window', function ($scope, $http, $window) {
        $scope.showAlert = false;
        $scope.showAlertPassword = false;

        $scope.model = {};

        $scope.getModel = function () {
            $http({
                method: 'GET',
                url: '/Account/GetManageModel'
            }).success(function (data, status, headers, config) {
                $scope.model = data;
                $scope.profileForm.$setPristine();
                $scope.passwordForm.$setPristine();
            }).error(function (data, status, headers, config) {
                $scope.message = 'Erro inesperado';
                $scope.messagetyoe = 'Erro';
                $scope.showAlert = true;
                $scope.showAlertPassword = false;
                $scope.hasError = true;
                $scope.success = false;
            });
        };

        $scope.getModel();

        // function to submit the form after all validation has occurred
        $scope.submitForm = function (isValid) {
            $scope.showAlert = false;
            $scope.showAlertPassword = false;
            $scope.hasError = false;
            $scope.success = false;

            if (isValid) {
                console.log($scope.antiForgeryToken);
                $http({
                    method: 'POST',
                    url: '/Account/Manage',
                    data: $scope.model,
                    headers: {
                        'RequestVerificationToken': $scope.antiForgeryToken
                    }
                }).success(function (data, status, headers, config) {
                    $scope.messagetype = '';
                    $scope.message = '';
                    $scope.messagepassword = '';
                    $scope.showAlert = false;
                    $scope.showAlertPassword = false;
                    $scope.hasError = false;
                    $scope.success = false;

                    if (data.success == false) {
                        $scope.messagetype = 'Erro';
                        $scope.hasError = true;
                        var str = '';
                        for (var error in data.errors) {
                            str += data.errors[error] + '\n';
                        }
                        if (!($scope.model.LocalPasswordModel.ConfirmPassword == null || $scope.model.LocalPasswordModel.ConfirmPassword === '')) {
                            $scope.messagepassword = str;
                            $scope.showAlertPassword = true;
                        }
                        else {
                            $scope.message = str;
                            $scope.showAlert = true;
                        }
                    }
                    else {
                        $scope.messagetype = 'Sucesso';
                        $scope.success = true;
                        if (!($scope.model.LocalPasswordModel.ConfirmPassword == null || $scope.model.LocalPasswordModel.ConfirmPassword === '')) {
                            $scope.messagepassword = data.message;
                            $scope.showAlertPassword = true;
                        }
                        else {
                            $scope.message = data.message;
                            $scope.showAlert = true;
                        }

                        $scope.getModel();
                    }
                }).error(function (data, status, headers, config) {
                    $scope.message = 'Erro inesperado';
                });
            }
        };
    }]);

    angular.module('loginApp').controller('registerCtrl', ['$scope', '$http', '$window', function ($scope, $http, $window) {
        $scope.showAlert = false;

        $scope.model = {};

        $scope.getModel = function () {
            $http({
                method: 'GET',
                url: '/Account/GetRegisterModel'
            }).success(function (data, status, headers, config) {
                $scope.model = data;
                $scope.registerForm.$setPristine();
            }).error(function (data, status, headers, config) {
                $scope.message = 'Erro inesperado';
                $scope.messagetype = 'Erro ' + data;
                $scope.showAlert = true;
                $scope.hasError = true;
                $scope.success = false;
            });
        };

        $scope.getModel();

        // function to submit the form after all validation has occurred
        $scope.submitForm = function (isValid) {
            $scope.showAlert = false;
            $scope.hasError = false;
            $scope.success = false;

            if (isValid) {
                $http({
                    method: 'POST',
                    url: '/Account/Register',
                    data: $scope.model,
                    headers: {
                        'RequestVerificationToken': $scope.antiForgeryToken
                    }
                }).success(function (data, status, headers, config) {
                    console.log(data);
                    console.log(status);
                    $scope.messagetype = '';
                    $scope.message = '';
                    $scope.showAlert = false;
                    $scope.hasError = false;
                    $scope.success = false;

                    if (data.success == false) {
                        $scope.messagetype = 'Erro';
                        $scope.hasError = true;
                        var str = '';
                        for (var error in data.errors) {
                            str += data.errors[error] + '\n';
                        }

                        $scope.message = str;
                        $scope.showAlert = true;
                    }
                    else {
                        $window.location.href = "/";
                    }
                }).error(function (data, status, headers, config) {
                    console.log(data);
                    console.log(status);
                    $scope.message = 'Erro inesperado';
                    $scope.showAlert = true;
                });
            }
        };
    }]);
})();