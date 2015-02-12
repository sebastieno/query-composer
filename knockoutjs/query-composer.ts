/// <reference path="./typings/knockout/knockout.d.ts" />
/// <reference path="./typings/knockout.viewmodel/knockout.viewmodel.d.ts" />

module QueryComposer {
    export module Model {
        /*
         * Type of the field
         */
        export enum FieldTypes {
            Text,
            List,
            Multiple
        }

        /*
         * Model representing a query
         */
        export class Query {
            /*
             * Field used for the query
             */
            public field: KnockoutObservable<FieldDefinition> = ko.observable(null);

            /*
             * Value used for the query
             */
            public value: KnockoutObservable<string> = ko.observable("");

            /*
             * Operator between this query and the previous one
             */
            public operator: KnockoutObservable<string> = ko.observable("");


            public dependantQuery: boolean = false;
        }

        /**
         * Represents a list value (used for the list field definition)
         */
        export class ListValue {
            public value: number;
            public text: string;

            constructor(value: number, text: string) {
                this.value = value;
                this.text = text;
            }
        }

        /*
         * Interface representing a field definition
         */
        export interface FieldDefinition {
            name: string;
            text: string;
            type: FieldTypes;
        }

        /*
         * Model representing a text field definition
         */
        export class TextFieldDefinition implements FieldDefinition {
            public name: string;
            public text: string;
            public type: FieldTypes = FieldTypes.Text;

            constructor(name: string, text: string) {
                this.name = name;
                this.text = text;
            }
        }

        /*
         * Model representing a list field definition
         */
        export class ListFieldDefinition implements FieldDefinition {
            public name: string;
            public text: string;
            public values: ListValue[];
            public type: FieldTypes = FieldTypes.List;

            constructor(name: string, text: string, values: ListValue[]) {
                this.name = name;
                this.text = text;
                this.values = values;
            }
        }

        /*
         * Model representing a multiple fields definition
         */
        export class MultipleFieldsDefinition implements FieldDefinition {
            public name: string;
            public text: string;
            public type: FieldTypes = FieldTypes.Multiple;

            public mainField: FieldDefinition;
            public childrenFields: FieldDefinition[];

            constructor(mainField: FieldDefinition, childrenFields: FieldDefinition[]) {
                this.name = mainField.name;
                this.text = mainField.text;

                this.mainField = mainField;
                this.childrenFields = childrenFields;
            }
        }
    }

    export class Configuration {
        public showNewEmptyLine: boolean;
    }

    /*
     * View model used to create queries composition
     */
    export class QueriesViewModel {
        /*
         * Queries of the composition
         */
        public queries: KnockoutObservableArray<Model.Query> = ko.observableArray([]);

        /*
         * List of fields definition
         */
        public fieldsDefinition: Model.FieldDefinition[];

        /*
         * Operators list, used between each query
         */
        public operators: any = [{ name: 'ET', value: '&&' }, { name: 'OU', value: '||' }];

        constructor(fieldsDefinition: Model.FieldDefinition[], configuration: Configuration, queries: any) {
            this.fieldsDefinition = fieldsDefinition;

            if (queries) {
                for (var i = 0; i < queries.length; i++) {
                    var query = new Model.Query();

                    var fields = this.fieldsDefinition.filter(function (field) {
                        return field.name == queries[i].field;
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

        changeQueryFieldDefinition(query: Model.Query): void {
            var index = this.queries.indexOf(query);

            this.queries.valueWillMutate();
            if (!query.field() || query.field().type != Model.FieldTypes.Multiple) {
                while (this.queries()[index + 1] && this.queries()[index + 1].dependantQuery) {
                    this.removeQuery(this.queries()[index + 1]);
                }
            }

            if (query.field() && query.field().type == Model.FieldTypes.Multiple) {
                var field: any = query.field();
                var multipleFieldsDefinition: Model.MultipleFieldsDefinition = field;

                this.queries.valueWillMutate();
                var fieldIndex = 1;
                multipleFieldsDefinition.childrenFields.forEach(childField => {
                    var query = new Model.Query();
                    query.operator("&&");
                    query.field(childField);
                    query.dependantQuery = true;

                    this.queries.splice(index + fieldIndex, 0, query);
                    fieldIndex++;
                });
            }

            this.queries.valueHasMutated();
        }

        addQuery(): void {
            this.queries.push(new Model.Query());
        }

        removeQuery(query: Model.Query): void {
            var index = this.queries.indexOf(query);

            this.queries.valueWillMutate();
            if (query.field().type == Model.FieldTypes.Multiple) {
                while (this.queries()[index + 1] && this.queries()[index + 1].dependantQuery) {
                    this.removeQuery(this.queries()[index + 1]);
                }
            }

            this.queries.remove(query);

            this.queries.valueHasMutated();
        }

        goUp(query: Model.Query): void {
            this.queries.valueWillMutate();

            var queriesToMove: Model.Query[] = [];
            var startIndex = this.queries.indexOf(query);
            queriesToMove.push(this.queries.splice(startIndex, 1)[0]);

            if (query.field() && query.field().type == Model.FieldTypes.Multiple) {
                while (this.queries()[startIndex] && this.queries()[startIndex].dependantQuery) {
                    queriesToMove.push(this.queries.splice(startIndex, 1)[0]);
                }
            }

            var targetIndex = startIndex - 1;
            while (this.queries()[targetIndex] && this.queries()[targetIndex].dependantQuery) {
                targetIndex--;
            }

            queriesToMove.forEach(query => {
                this.queries.splice(targetIndex, 0, query);
                targetIndex++;
            });

            this.queries.valueHasMutated();
        }

        goDown(query: Model.Query): void {
            this.queries.valueWillMutate();

            var queriesToMove: Model.Query[] = [];
            var startIndex = this.queries.indexOf(query);
            queriesToMove.push(this.queries.splice(startIndex, 1)[0]);

            if (query.field() && query.field().type == Model.FieldTypes.Multiple) {
                while (this.queries()[startIndex] && this.queries()[startIndex].dependantQuery) {
                    queriesToMove.push(this.queries.splice(startIndex, 1)[0]);
                }
            }

            var targetIndex = startIndex + 1;
            while (this.queries()[targetIndex] && this.queries()[targetIndex].dependantQuery) {
                targetIndex++;
            }

            queriesToMove.forEach(query => {
                this.queries.splice(targetIndex, 0, query);
                targetIndex++;
            });

            this.queries.valueHasMutated();
        }
    }
}