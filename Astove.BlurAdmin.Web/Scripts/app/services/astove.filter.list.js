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

    angular.module('AstoveApp').service('AstoveFilterList', ['AstoveService', 'AstoveDataList', 'AstoveCommon', '$location', '$q',
            function ($service, $data, $global, $location, $q) {
                this.data = $data;
                
                this.addFilter = function ($scope) {
                    if ($data.condition.filterType.indexOf('DateFilter') > -1) {
                        $data.condition.defaultOperator = '4';
                        $data.condition.displayOperator = '[maior ou igual]';
                        if ($data.condition.defaultValue !== '') {
                            $data.condition.defaultValue = new Date($data.condition.defaultValue).toUTCString();
                            $data.entity.filterConditions.push($data.condition);
                        }
                        var condition = _.clone($data.condition);
                        condition.defaultOperator = '5';
                        condition.displayOperator = '[menor]';
                        condition.index = $data.condition.index+'.1';
                        condition.defaultValue = $data.condition.defaultValue2;
                        if (condition.defaultValue !== '') {
                            condition.defaultValue = new Date(condition.defaultValue).toUTCString();
                            condition.displayValue = new Date(condition.defaultValue).toLocaleDateString();
                            $data.entity.filterConditions.push(condition);
                        }                            
                    }
                    else {
                        $data.entity.filterConditions.push($data.condition);
                    }
                    $data.clearCondition();
                    this.filtrar($scope);
                };

                this.addModalFilter = function ($scope) {
                    JSON.stringify($data.modalCondition);
                    $data.modalEntity.filterConditions.push($data.modalCondition);
                    $data.clearModalCondition();
                    this.filtrarModal($scope);
                };

                this.addFilterCondition = function ($scope, condition) {
                    this.clearFilterConditions();
                    $data.entity.filterConditions.push(condition);
                    this.filtrar($scope);
                };

                this.addModalFilterCondition = function ($scope, condition) {
                    this.clearModalFilterConditions();
                    $data.modalEntity.filterConditions.push(modalCondition);
                    this.filtrarModal($scope);
                };

                this.removeFilter = function ($scope, condition) {
                    var index = $data.entity.filterConditions.indexOf(condition);
                    $data.entity.filterConditions.splice(index, 1);
                    this.filtrar($scope);
                };

                this.removeModalFilter = function ($scope, condition) {
                    var index = $data.modalEntity.filterConditions.indexOf(condition);
                    $data.modalEntity.filterConditions.splice(index, 1);
                    this.filtrarModal($scope);
                };

                this.setFilterCondition = function (condition) {
                    $data.condition = condition;
                };

                this.setModalFilterCondition = function (condition) {
                    $data.modalCondition = condition;
                };

                this.clearFilterConditions = function () {
                    for (var i = 0; i < $data.entity.filterConditions.length; i++) {
                        $data.entity.filterConditions.splice(i, 1);
                    }
                };

                this.clearModalFilterConditions = function () {
                    for (var i = 0; i < $data.modalEntity.filterConditions.length; i++) {
                        $data.modalEntity.filterConditions.splice(i, 1);
                    }
                };

                this.paginate = function ($scope, page) {
                    $data.entity.pagingOptions.pageIndex = page;
                    this.filtrar($scope);
                };

                this.paginateModal = function ($scope, page) {
                    $data.modalEntity.pagingOptions.pageIndex = page;
                    this.filtrarModal($scope);
                };

                this.filtrar = function ($scope, params) {
                    $global.loading = true;
                    $data.parseFilterConditions(params);
                    $data.parseSortOptions($scope, params);
                    $service.reloadDataList($scope, params);
                };

                this.filtrarAsync = function ($scope, params) {
                    var deferred = $q.defer();

                    $global.loading = true;
                    $data.parseFilterConditions(params);
                    $data.parseSortOptions($scope, params);
                    
                    var promise = $service.reloadDataListAsync($scope, params);
                    promise.then(function (response) {
                        deferred.resolve(response);
                    }, function (reason) {
                        deferred.reject(reason);
                    });

                    return deferred.promise;
                };

                this.filtrarModal = function ($scope, params) {
                    $global.loading = true;
                    $data.parseModalFilterConditions(params);
                    $data.parseModalSortOptions($scope, params);
                    $service.reloadModalDataList($scope, params);
                };

                this.listen = function ($scope) {
                    var self = this;
                    $scope.$watch('data.parentKey', function (newVal, oldVal) {
                        if (newVal !== oldVal && !$data.firstExecution) {
                            self.filtrar($scope);
                        }
                    }, true);
                    this.listenUrl($scope);
                };

                this.listenModal = function ($scope) {
                    var self = this;
                    $scope.$watch('data.modalParentKey', function (newVal, oldVal) {
                        if (newVal !== oldVal && !$data.firstModalExecution) {
                            self.filtrarModal($scope);
                        }
                    }, true);
                    this.listenModalUrl($scope);
                };

                this.listenUrl = function ($scope) {
                    var self = this;
                    $scope.$watch(function () {
                        return $location.url();
                    }, function () {
                        if (!$data.firstExecution) {
                            var params = $location.search();
                            self.filtrar($scope, params);
                        }
                    });
                };

                this.listenModalUrl = function ($scope) {
                    var self = this;
                    $scope.$watch(function () {
                        return $location.url();
                    }, function () {
                        if (!$data.firstModalExecution) {
                            var params = $location.search();
                            self.filtrarModal($scope, params);
                        }
                    });
                };
            } ]);

})();