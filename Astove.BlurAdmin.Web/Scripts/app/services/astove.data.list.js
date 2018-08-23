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

    angular.module('AstoveApp').service('AstoveDataList', ['AstoveCommon', 
        function ($global) {
            $global.loading = false;
            this.condition = {};
            this.modalCondition = {};
            this.entity = {};
            this.modalEntity = {};
            this.model = {};
            this.modalModel = {};
            this.totalServerItems = 0;
            this.totalModalServerItems = 0;
            this.parentKey = '';
            this.parentValue = '';
            this.modalParentKey = '';
            this.modalParentValue = '';
            this.columnsToSort = ['Id'];
            this.modalColumnsToSort = ['Id'];
            this.directionsToSort = ['asc'];
            this.modalDirectionsToSort = ['asc'];
            this.firstExecution = true;
            this.firstModalExecution = true;
            this.pagingOptions = {
                pageSizes: [],
                pageSize: 12,
                pageIndex: 1,
                totalCount: 0,
                totalPageCount: 0,
                hasNextPage: false,
                hasPreviousPage: false
            };
            this.modalPagingOptions = {
                pageSizes: [],
                pageSize: 12,
                pageIndex: 1,
                totalCount: 0,
                totalPageCount: 0,
                hasNextPage: false,
                hasPreviousPage: false
            };

            this.sortOptions = {};
            this.modalSortOptions = {};

            this.sortBy = function ($scope, field, defaultSort, isModal) {
                isModal = (typeof isModal === 'undefined') ? false : isModal;
                if (!isModal) {
                    this.addToSortBy($scope, field, this.columnsToSort, this.directionsToSort, defaultSort);
                }
                else {
                    this.addToSortBy($scope, field, this.modalColumnsToSort, this.modalDirectionsToSort, defaultSort);
                }
            };

            this.addToSortBy = function ($scope, field, sorts, directions, defaultSort) {
                var indexOf = _.indexOf(sorts, field);
                if (indexOf === -1) {
                    sorts.push(field);
                    directions.push('asc');
                }
                else {
                    var direction = directions[indexOf];
                    if (direction === 'desc') {
                        sorts.splice(indexOf, 1);
                        directions.splice(indexOf, 1);
                        var fieldName = 'class' + field;
                        $scope[fieldName] = '';
                    }
                    else {
                        directions[indexOf] = 'desc';
                    }
                }
            };

            this.getDirection = function (field, isModal) {
                isModal = (typeof isModal === 'undefined') ? false : isModal;
                if (!isModal) {
                    return this.getDirectionInternal(field, this.columnsToSort, this.directionsToSort);
                }
                else {
                    return this.getDirectionInternal(field, this.modalColumnsToSort, this.modalDirectionsToSort);
                }
            };
            
            this.getDirectionInternal = function (field, sorts, directions) {
                var indexOf = _.indexOf(sorts, field);
                if (indexOf === -1) {
                    return -1;
                }
                else {
                    var direction = directions[indexOf];
                    if (direction === 'desc') {
                        return 1;
                    }
                    else {
                        return 0;
                    }
                };
            };

            this.setRowSortClass = function ($scope, field) {
                var fieldName = 'class' + field;
                $scope[fieldName] = '';
                var direction = this.getDirection(field);                
                if (direction === 0) {
                    $scope[fieldName] = 'fa fa-caret-up';
                }
                else {
                    $scope[fieldName] = 'fa fa-caret-down';
                }
            };

            this.clear = function () {
                this.firstExecution = true;
                this.clearCondition();
                this.entity.filterConditions = [];
                this.entity.filtersType = [];
                this.entity.fields = [];
                this.entity.operators = [];
                this.entity.values = [];
                this.entity.items = [];
                this.entity.pagingOptions = {
                    pageSizes: [],
                    pageSize: 12,
                    pageIndex: 1,
                    totalCount: 0,
                    totalPageCount: 0,
                    hasNextPage: false,
                    hasPreviousPage: false
                };
                this.sortOptions = {};
            };

            this.clearModal = function () {
                this.firstModalExecution = true;
                this.clearModalCondition();
                this.modalEntity.filterConditions = [];
                this.modalEntity.filtersType = [];
                this.modalEntity.fields = [];
                this.modalEntity.operators = [];
                this.modalEntity.values = [];
                this.modalEntity.items = [];
                this.modalEntity.pagingOptions = {
                    pageSizes: [],
                    pageSize: 12,
                    pageIndex: 1,
                    totalCount: 0,
                    totalPageCount: 0,
                    hasNextPage: false,
                    hasPreviousPage: false
                };
                this.modalSortOptions = {};
            };

            this.clearCondition = function () {
                this.condition = {
                    index: '',
                    property: '',
                    displayName: '',
                    defaultOperator: '',
                    displayOperator: '',
                    operators: [],
                    defaultValue: '',
                    defaultValue2: '',
                    domainValues: [],
                    displayValue: '',
                    filterType: '',
                    exists: '',
                    mask: '',
                    css: '',
                    width: 0,
                    length: 0
                };
            };

            this.clearModalCondition = function () {
                this.modalCondition = {
                    index: '',
                    property: '',
                    displayName: '',
                    defaultOperator: '',
                    displayOperator: '',
                    operators: [],
                    defaultValue: '',
                    defaultValue2: '',
                    domainValues: [],
                    displayValue: '',
                    filterType: '',
                    exists: '',
                    mask: '',
                    css: '',
                    width: 0,
                    length: 0
                };
            };

            this.parseFilterConditions = function (params) {
                this.entity.hasDefaultConditions = false;

                if ($global.enableQueryString && typeof params !== 'undefined' && !_.isEmpty(params)) {
                    if (typeof params.filtersType !== 'undefined')
                        this.entity.filtersType = params.filtersType;

                    if (typeof params.fields !== 'undefined')
                        this.entity.fields = params.fields;

                    if (typeof params.operators !== 'undefined')
                        this.entity.operators = params.operators;

                    if (typeof params.values !== 'undefined')
                        this.entity.values = params.values;
                }
                else if (typeof this.entity.filterConditions !== 'undefined') {
                    this.entity.filtersType = new Array(this.entity.filterConditions.length);
                    this.entity.fields = new Array(this.entity.filterConditions.length);
                    this.entity.operators = new Array(this.entity.filterConditions.length);
                    this.entity.values = new Array(this.entity.filterConditions.length);

                    for (var i = 0; i < this.entity.filterConditions.length; i++) {
                        this.entity.filtersType[i] = this.entity.filterConditions[i].filterType;
                        this.entity.fields[i] = this.entity.filterConditions[i].property;
                        this.entity.operators[i] = this.entity.filterConditions[i].defaultOperator;
                        this.entity.values[i] = decodeURIComponent(this.entity.filterConditions[i].defaultValue);
                    }
                }
            };

            this.parseModalFilterConditions = function (params) {
                this.modalEntity.hasDefaultConditions = false;

                if ($global.enableQueryString && typeof params !== 'undefined' && !_.isEmpty(params)) {
                    if (typeof params.filtersType !== 'undefined')
                        this.modalEntity.filtersType = params.filtersType;

                    if (typeof params.fields !== 'undefined')
                        this.modalEntity.fields = params.fields;

                    if (typeof params.operators !== 'undefined')
                        this.modalEntity.operators = params.operators;

                    if (typeof params.values !== 'undefined')
                        this.modalEntity.values = params.values;
                }
                else if (typeof this.modalEntity.filterConditions !== 'undefined') {
                    this.modalEntity.filtersType = new Array(this.modalEntity.filterConditions.length);
                    this.modalEntity.fields = new Array(this.modalEntity.filterConditions.length);
                    this.modalEntity.operators = new Array(this.modalEntity.filterConditions.length);
                    this.modalEntity.values = new Array(this.modalEntity.filterConditions.length);

                    for (var i = 0; i < this.modalEntity.filterConditions.length; i++) {
                        this.modalEntity.filtersType[i] = this.modalEntity.filterConditions[i].filterType;
                        this.modalEntity.fields[i] = this.modalEntity.filterConditions[i].property;
                        this.modalEntity.operators[i] = this.modalEntity.filterConditions[i].defaultOperator;
                        this.modalEntity.values[i] = decodeURIComponent(this.modalEntity.filterConditions[i].defaultValue);
                    }
                }
            };

            this.parseSortOptions = function ($scope, params) {
                if ($global.enableQueryString && typeof params !== 'undefined' && !_.isEmpty(params)) {
                    if (typeof params.ordersBy !== 'undefined' && typeof params.directions !== 'undefined') {
                        this.entity.sortOptions = {
                            fields: params.ordersBy,
                            directions: params.directions
                        };
                    }
                }
                else if (this.columnsToSort.length > 0) {
                    this.entity.sortOptions = {
                        fields: new Array(this.columnsToSort.length),
                        directions: new Array(this.directionsToSort.length)
                    };

                    for (var i = 0; i < this.columnsToSort.length; i++) {
                        this.entity.sortOptions.fields[i] = this.columnsToSort[i];
                        this.entity.sortOptions.directions[i] = this.directionsToSort[i];
                        this.setRowSortClass($scope, this.columnsToSort[i]);
                    }
                }
            };

            this.parseModalSortOptions = function ($scope, params) {
                if ($global.enableQueryString && typeof params !== 'undefined' && !_.isEmpty(params)) {
                    if (typeof params.ordersBy !== 'undefined' && typeof params.directions !== 'undefined') {
                        this.modalEntity.sortOptions = {
                            fields: params.ordersBy,
                            directions: params.directions
                        };
                    }
                }
                else if (this.modalColumnsToSort.length > 0) {
                    this.modalEntity.sortOptions = {
                        fields: new Array(this.modalColumnsToSort.length),
                        directions: new Array(this.modalDirectionsToSort.length)
                    };

                    for (var i = 0; i < this.modalColumnsToSort.length; i++) {
                        this.modalEntity.sortOptions.fields[i] = this.modalColumnsToSort[i];
                        this.modalEntity.sortOptions.directions[i] = this.modalDirectionsToSort[i];
                        this.setRowSortClass($scope, this.modalDirectionsToSort[i]);

                    }
                }
            };

            this.setDisplayOperator = function (defaultOperator) {
                var operator = _.findWhere(this.entity.filterOptions.filters[this.condition.index].operators, { key: this.condition.defaultOperator });
                this.condition.displayOperator = '';
                if (typeof operator !== 'undefined')
                    this.condition.displayOperator = operator.value;
            };

            this.setDisplayModalOperator = function (defaultOperator) {
                var operator = _.findWhere(this.modalEntity.filterOptions.filters[this.modalCondition.index].operators, { key: this.modalCondition.defaultOperator });
                this.modalCondition.displayOperator = '';
                if (typeof operator !== 'undefined')
                    this.modalCondition.displayOperator = operator.value;
            };

            this.setDefaultValue = function (defaultValue) {
                if (this.entity.filterOptions.filters[this.condition.index].filterType === 'DateFilter')
                    this.condition.displayValue = new Date(defaultValue).toLocaleDateString();
                else if (this.entity.filterOptions.filters[this.condition.index].domainValues !== null) {
                    var domainValue = _.findWhere(this.entity.filterOptions.filters[this.condition.index].domainValues, { key: this.condition.defaultValue });
                    this.condition.displayValue = '';
                    if (typeof domainValue !== 'undefined')
                        this.condition.displayValue = domainValue.value;
                }
                else
                    this.condition.displayValue = defaultValue;
            };

            this.setDefaultModalValue = function (defaultValue) {
                if (this.modalEntity.filterOptions.filters[this.modalCondition.index].filterType === 'DateFilter')
                    this.modalCondition.displayValue = new Date(defaultValue).toLocaleDateString();
                else if (this.modalEntity.filterOptions.filters[this.modalCondition.index].domainValues !== null) {
                    var domainValue = _.findWhere(this.modalEntity.filterOptions.filters[this.modalCondition.index].domainValues, { key: this.modalCondition.defaultValue });
                    this.modalCondition.displayValue = '';
                    if (typeof domainValue !== 'undefined')
                        this.modalCondition.displayValue = domainValue.value;
                }
                else
                    this.modalCondition.displayValue = defaultValue;
            };

            this.onSelectedIndex = function (index) {
                this.condition.property = this.entity.filterOptions.filters[this.condition.index].property;
                this.condition.displayName = this.entity.filterOptions.filters[this.condition.index].displayName;
                this.condition.filterType = this.entity.filterOptions.filters[this.condition.index].filterType;
                this.condition.defaultOperator = this.entity.filterOptions.filters[this.condition.index].defaultOperator;
                this.condition.operators = this.entity.filterOptions.filters[this.condition.index].operators;
                if (this.condition.defaultOperator !== '') {
                    this.setDisplayOperator(this.condition.defaultOperator);
                }
                this.condition.defaultValue = this.entity.filterOptions.filters[this.condition.index].defaultValue;
                this.condition.domainValues = this.entity.filterOptions.filters[this.condition.index].domainValues;
                if (this.condition.defaultValue !== '') {
                    this.setDefaultValue(this.condition.defaultValue);
                }
                this.condition.mask = this.entity.filterOptions.filters[this.condition.index].mask;
            };

            this.onSelectedModalIndex = function (index) {
                this.modalCondition.property = this.modalEntity.filterOptions.filters[this.modalCondition.index].property;
                this.modalCondition.displayName = this.modalEntity.filterOptions.filters[this.modalCondition.index].displayName;
                this.modalCondition.filterType = this.modalEntity.filterOptions.filters[this.modalCondition.index].filterType;
                this.modalCondition.defaultOperator = this.modalEntity.filterOptions.filters[this.modalCondition.index].defaultOperator;
                this.modalCondition.operators = this.modalEntity.filterOptions.filters[this.modalCondition.index].operators;
                if (this.modalCondition.defaultOperator !== '') {
                    this.setDisplayOperator(this.modalCondition.defaultOperator);
                }
                this.modalCondition.defaultValue = this.modalEntity.filterOptions.filters[this.modalCondition.index].defaultValue;
                this.modalCondition.domainValues = this.modalEntity.filterOptions.filters[this.modalCondition.index].domainValues;
                if (this.modalCondition.defaultValue !== '') {
                    this.setDefaultValue(this.modalCondition.defaultValue);
                }
                this.modalCondition.mask = this.modalEntity.filterOptions.filters[this.modalCondition.index].mask;
            };

            this.listen = function ($scope) {
                var self = this;
                if (self.condition !== null) {
                    $scope.$watch('data.condition', function (newVal, oldVal) {
                        if (self.condition.index === '') {
                            self.clearCondition();
                        }
                        else if (newVal !== oldVal && newVal.index !== oldVal.index) {
                            self.onSelectedIndex(newVal.index);
                        }
                        else if (newVal !== oldVal && newVal.defaultOperator !== oldVal.defaultOperator) {
                            self.setDisplayOperator(newVal.defaultOperator);
                        }
                        else if (newVal !== oldVal && newVal.defaultValue !== oldVal.defaultValue) {
                            self.setDefaultValue(newVal.defaultValue);
                        }
                    }, true);
                }
            };

            this.listenModal = function ($scope) {
                var self = this;
                if (self.condition !== null) {
                    $scope.$watch('data.modalCondition', function (newVal, oldVal) {
                        if (self.modalCondition.index === '') {
                            self.clearModalCondition();
                        }
                        else if (newVal !== oldVal && newVal.index !== oldVal.index) {
                            self.onSelectedModalIndex(newVal.index);
                        }
                        else if (newVal !== oldVal && newVal.defaultOperator !== oldVal.defaultOperator) {
                            self.setDisplayModalOperator(newVal.defaultOperator);
                        }
                        else if (newVal !== oldVal && newVal.defaultValue !== oldVal.defaultValue) {
                            self.setDefaultModalValue(newVal.defaultValue);
                        }
                    }, true);
                }
            };
        } ]);

})();