/// <reference path="./typings/knockout/knockout.d.ts" />
/// <reference path="./typings/knockout.viewmodel/knockout.viewmodel.d.ts" />

module QueryComposer {
    export module Model {
        /*
         * Type of the query
         */
        export enum QueryType {
            Simple,
            Multiple
        }

        /*
         * Type of the field
         */
        export enum FieldType {
            Text,
            List
        }

        /*
         * Model representing a query
         */
        export class Query {
            /*
             * Field used for the query
             */
            public queryDefinition: KnockoutObservable<any> = ko.observable(null);

            /*
             * Value used for the query
             */
            public value: KnockoutObservable<string> = ko.observable("");

            /*
             * Operator between this query and the previous one
             */
            public operator: KnockoutObservable<string> = ko.observable("");
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
         * Interface representing a query definition
         */
        export interface QueryDefinition {
            type: QueryType;
            text: string;
        }

        export class SimpleQueryDefinition implements QueryDefinition {
            public type: QueryType = QueryType.Simple;
            public field: FieldDefinition;
            public text: string;

            constructor(field: FieldDefinition) {
                this.field = field;
                this.text = this.field.text;
            }
        }

        export class MultipleQueryDefinition implements QueryDefinition {
            public type: QueryType = QueryType.Multiple;
            public fields: FieldDefinition[];
            public text: string;

            constructor(fields: FieldDefinition[]) {
                this.fields = fields;
                this.text = this.fields[0].text;
            }
        }

        /*
         * Interface representing a field definition
         */
        export interface FieldDefinition {
            name: string;
            text: string;
            type: FieldType;
        }

        /*
         * Model representing a text field definition
         */
        export class TextFieldDefinition implements FieldDefinition {
            public name: string;
            public text: string;
            public type: FieldType = FieldType.Text;

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
            public type: FieldType = FieldType.List;

            constructor(name: string, text: string, values: ListValue[]) {
                this.name = name;
                this.text = text;
                this.values = values;
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
         * List of queries definition
         */
        public queriesDefinition: Model.QueryDefinition[];

        /*
         * Operators list, used between each query
         */
        public operators: any = [{ name: 'ET', value: '&&' }, { name: 'OU', value: '||' }];

        constructor(queriesDefinition: Model.QueryDefinition[], configuration: Configuration, queries: any) {
            this.queriesDefinition = queriesDefinition;

            //if (queries) {
            //    for (var i = 0; i < queries.length; i++) {
            //        var query = new Model.Query();

            //        var fields = this.fieldsDefinition.filter(function (field) {
            //            return true; // TODO: field.name == queries[i].field;
            //        });

            //        //if (fields.length === 1) {
            //        //    query.field(fields[0]);

            //        //    if (this.queries().length == 0) {
            //        //        query.operator("&&");
            //        //    } else {
            //        //        query.operator(queries[i].operator);
            //        //    }

            //        //    query.value(queries[i].value);

            //        //    this.queries.push(query);
            //        //} else {
            //        //    console.warn("The field " + queries[i].field + " cannot be retrieved from the fields definition");
            //        //}
            //    }
            //}

            if (configuration && configuration.showNewEmptyLine) {
                this.addQuery();
            }
        }

        addQuery(): void {
            this.queries.push(new Model.Query());
        }

        removeQuery(query: Model.Query): void {
            this.queries.remove(query);
        }

        goUp(query: Model.Query): void {
            var currentIndex = this.queries.indexOf(query);
            var targetIndex = currentIndex - 1;

            if (targetIndex < 0) return;

            var currentElement = this.queries()[targetIndex];

            this.queries.valueWillMutate();
            this.queries()[currentIndex] = currentElement;
            this.queries()[targetIndex] = query;
            this.queries.valueHasMutated();
        }

        goDown(query: Model.Query): void {
            var currentIndex = this.queries.indexOf(query);
            var targetIndex = currentIndex + 1;

            if (targetIndex >= this.queries().length) return;

            var currentElement = this.queries()[targetIndex];

            this.queries.valueWillMutate();
            this.queries()[currentIndex] = currentElement;
            this.queries()[targetIndex] = query;
            this.queries.valueHasMutated();
        }
    }
}