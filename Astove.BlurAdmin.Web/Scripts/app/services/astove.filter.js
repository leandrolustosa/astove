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

    angular.module('AstoveApp').service('AstoveFilter', ['AstoveService', 'AstoveData', 'AstoveCommon',
            function ($service, $data, $global) {
                this.addFilter = function ($scope, propertyName) {
                    $data.entity[propertyName].filterConditions.push($data.condition[propertyName]);
                    $data.clearCondition(propertyName);
                    this.filtrar($scope, propertyName);
                };

                this.addFilterCondition = function ($scope, propertyName, condition) {
                    this.clearFilterConditions(propertyName);
                    $data.entity[propertyName].filterConditions.push(condition);
                    this.filtrar($scope, propertyName);
                };

                this.removeFilter = function ($scope, propertyName, condition) {
                    var index = $data.entity[propertyName].filterConditions.indexOf(condition);
                    $data.entity[propertyName].filterConditions.splice(index, 1);
                    this.filtrar($scope, propertyName);
                };

                this.setFilterCondition = function (condition) {
                    $data.condition = condition;
                };

                this.clearFilterConditions = function (propertyName) {
                    for (var i = 0; i < $data.entity[propertyName].filterConditions.length; i++) {
                        $data.entity[propertyName].filterConditions.splice(i, 1);
                    }
                };

                this.paginate = function ($scope, propertyName, page) {
                    console.log(propertyName);
                    console.log(page);
                    $data.entity[propertyName].pagingOptions.pageIndex = page;
                    this.filtrar($scope, propertyName);
                };

                this.filtrar = function ($scope, propertyName) {
                    $global.loading = true;
                    $data.parseFilterConditions(propertyName);
                    $service.reloadData($scope, propertyName);
                };
            } ]);

})();