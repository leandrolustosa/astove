(function () {
    "use strict";

    angular.module('codeApp').controller('codeCtrl', ['$scope', '$http', '$sce', function ($scope, $http, $sce) {
        $scope.model = {
            html: ''
        };
        $scope.refresh = false;

        $scope.getModel = function () {
            $http({
                method: 'GET',
                url: 'api/ac/Code/GetModel'
            }).success(function (data, status, headers, config) {
                $scope.model = data.model;
                $scope.codeForm.$setPristine();
            }).error(function (data, status, headers, config) {
                $scope.message = 'Erro inesperado';
                $scope.messagetype = 'Erro';
                $scope.hasError = true;
                $scope.success = false;
            });
        };

        $scope.getModel();

        $scope.updateModels = function () {
            $scope.model.postModel.groupId = $scope.model.postModel.groupKV.value;
            $http({
                method: 'GET',
                url: '/api/ac/Code/GetModels/' + $scope.model.postModel.groupId
            }).success(function (data, status, headers, config) {
                if (data.success === true) {
                    $scope.model.models = data.models;
                }
            }).error(function (data, status, headers, config) {
                $scope.messagetype = 'Erro';
                $scope.hasError = true;
                $scope.message = 'Erro inesperado';
            });
        };

        $scope.updateFields = function () {
            if ($scope.model.postModel.screenId === 'Insert/Update') {
                var groupNameTemplate = _.template("<%= groupName %>Form");
                var group = _.findWhere($scope.model.groups.items, { value: $scope.model.postModel.groupId });
                var groupName = (typeof group !== 'undefined') ? group.key : $scope.model.postModel.groupId;
                $scope.model.postModel.formName = groupNameTemplate({ groupName: groupName.toLowerCase() });
                $scope.model.postModel.submitMethod = "submitForm";
                $scope.model.postModel.buttonText = "Enviar";
            }                    
        };

        // function to submit the form after all validation has occurred
        $scope.submitForm = function (isValid) {
            $scope.hasError = false;
            $scope.success = false;

            if (isValid) {
                $http({
                    method: 'POST',
                    url: '/api/ac/Code/GenerateHtml',
                    data: $scope.model.postModel
                }).success(function (data, status, headers, config) {
                    $scope.messagetype = '';
                    $scope.message = '';
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
                    }
                    else {
                        $scope.messagetype = 'Sucesso';
                        $scope.success = true;
                        $scope.refresh = true;
                        $scope.message = data.message;
                        $scope.model = data.model;
                        $scope.codeForm.$setPristine();
                    }
                }).error(function (data, status, headers, config) {
                    $scope.messagetype = 'Erro';
                    $scope.hasError = true;
                    $scope.message = 'Erro inesperado';
                });
            }
        };
    }]);
})();