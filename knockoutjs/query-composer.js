/// <reference path="./typings/knockout/knockout.d.ts" />
/// <reference path="./typings/knockout.viewmodel/knockout.viewmodel.d.ts" />
var QueryComposer;
(function (QueryComposer) {
    (function (Model) {
        /*
        * Type of the field
        */
        (function (FieldTypes) {
            FieldTypes[FieldTypes["Text"] = 0] = "Text";
            FieldTypes[FieldTypes["List"] = 1] = "List";
            FieldTypes[FieldTypes["Multiple"] = 2] = "Multiple";
        })(Model.FieldTypes || (Model.FieldTypes = {}));
        var FieldTypes = Model.FieldTypes;

        /*
        * Model representing a query
        */
        var Query = (function () {
            function Query() {
                /*
                * Field used for the query
                */
                this.field = ko.observable(null);
                /*
                * Value used for the query
                */
                this.value = ko.observable("");
                /*
                * Operator between this query and the previous one
                */
                this.operator = ko.observable("");
            }
            return Query;
        })();
        Model.Query = Query;

        /**
        * Represents a list value (used for the list field definition)
        */
        var ListValue = (function () {
            function ListValue(value, text) {
                this.value = value;
                this.text = text;
            }
            return ListValue;
        })();
        Model.ListValue = ListValue;

        

        /*
        * Model representing a text field definition
        */
        var TextFieldDefinition = (function () {
            function TextFieldDefinition(name, text) {
                this.type = 0 /* Text */;
                this.name = name;
                this.text = text;
            }
            return TextFieldDefinition;
        })();
        Model.TextFieldDefinition = TextFieldDefinition;

        /*
        * Model representing a list field definition
        */
        var ListFieldDefinition = (function () {
            function ListFieldDefinition(name, text, values) {
                this.type = 1 /* List */;
                this.name = name;
                this.text = text;
                this.values = values;
            }
            return ListFieldDefinition;
        })();
        Model.ListFieldDefinition = ListFieldDefinition;

        /*
        * Model representing a field composed of a main field, and some childs fields
        */
        var MultipleFieldsDefinition = (function () {
            //public subQueries: Query[] = [];
            function MultipleFieldsDefinition(mainField, childrenFields) {
                this.type = 2 /* Multiple */;
                this.mainField = mainField;
                this.name = mainField.name;
                this.text = mainField.text;
                this.childrenFields = childrenFields;
                //childrenFields.forEach(field => {
                //    var query = new Query();
                //    query.field(field);
                //    this.subQueries.push(query);
                //});
            }
            return MultipleFieldsDefinition;
        })();
        Model.MultipleFieldsDefinition = MultipleFieldsDefinition;
    })(QueryComposer.Model || (QueryComposer.Model = {}));
    var Model = QueryComposer.Model;

    var Configuration = (function () {
        function Configuration() {
        }
        return Configuration;
    })();
    QueryComposer.Configuration = Configuration;

    /*
    * View model used to create queries composition
    */
    var QueriesViewModel = (function () {
        function QueriesViewModel(fieldsDefinition, configuration, queries) {
            /*
            * Queries of the composition
            */
            this.queries = ko.observableArray([]);
            /*
            * Operators list, used between each query
            */
            this.operators = [{ name: 'ET', value: '&&' }, { name: 'OU', value: '||' }];
            this.fieldsDefinition = fieldsDefinition;

            if (queries) {
                for (var i = 0; i < queries.length; i++) {
                    var query = new Model.Query();

                    var fields = this.fieldsDefinition.filter(function (field) {
                        return true;
                    });

                    if (fields.length === 1) {
                        query.field(fields[0]);

                        if (this.queries().length == 0) {
                            query.operator("&&");
                        } else {
                            query.operator(queries[i].operator);
                        }

                        query.value(queries[i].value);

                        this.queries.push(query);
                    } else {
                        console.warn("The field " + queries[i].field + " cannot be retrieved from the fields definition");
                    }
                }
            }

            if (configuration && configuration.showNewEmptyLine) {
                this.addQuery();
            }
        }
        QueriesViewModel.prototype.addQuery = function () {
            this.queries.push({});
        };

        QueriesViewModel.prototype.removeQuery = function (query) {
            this.queries.remove(query);
        };

        QueriesViewModel.prototype.goUp = function (query) {
            var currentIndex = this.queries.indexOf(query);
            var targetIndex = currentIndex - 1;

            if (targetIndex < 0)
                return;

            var currentElement = this.queries()[targetIndex];

            this.queries.valueWillMutate();
            this.queries()[currentIndex] = currentElement;
            this.queries()[targetIndex] = query;
            this.queries.valueHasMutated();
        };

        QueriesViewModel.prototype.goDown = function (query) {
            var currentIndex = this.queries.indexOf(query);
            var targetIndex = currentIndex + 1;

            if (targetIndex >= this.queries().length)
                return;

            var currentElement = this.queries()[targetIndex];

            this.queries.valueWillMutate();
            this.queries()[currentIndex] = currentElement;
            this.queries()[targetIndex] = query;
            this.queries.valueHasMutated();
        };
        return QueriesViewModel;
    })();
    QueryComposer.QueriesViewModel = QueriesViewModel;
})(QueryComposer || (QueryComposer = {}));
//# sourceMappingURL=query-composer.js.map
