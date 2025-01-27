//----------------------
// <auto-generated>
//     Generated using the NSwag toolchain v11.1.0.0 (Newtonsoft.Json v13.0.0.0) (http://NJsonSchema.org)
// </auto-generated>
//----------------------







export enum SchemaType {
    Menu = "menu",
    Entity = "entity",
    Query = "query",
    Page = "page",
}

export interface Settings {
    entity: Entity | undefined;
    query: Query | undefined;
    menu: Menu | undefined;
    page: Page | undefined;
}

export interface Entity {
    attributes: Attribute[];
    name: string;
    displayName: string;
    tableName: string;
    labelAttributeName: string;
    primaryKey: string;
    defaultPageSize: number;
    defaultPublicationStatus: PublicationStatus;
}

export interface Attribute {
    field: string;
    header: string;
    dataType: DataType;
    displayType: DisplayType;
    inList: boolean;
    inDetail: boolean;
    isDefault: boolean;
    options: string;
    validation: string;
}

export enum DataType {
    Int = "int",
    Datetime = "datetime",
    Text = "text",
    String = "string",
    Lookup = "lookup",
    Junction = "junction",
    Collection = "collection",
}

export enum DisplayType {
    Text = "text",
    Textarea = "textarea",
    Editor = "editor",
    Number = "number",
    Datetime = "datetime",
    Date = "date",
    Image = "image",
    Gallery = "gallery",
    File = "file",
    Dropdown = "dropdown",
    Multiselect = "multiselect",
    Lookup = "lookup",
    TreeSelect = "treeSelect",
    Picklist = "picklist",
    Tree = "tree",
    EditTable = "editTable",
}

export enum PublicationStatus {
    Draft = "draft",
    Published = "published",
    Unpublished = "unpublished",
    Scheduled = "scheduled",
}

export interface Query {
    name: string;
    entityName: string;
    source: string;
    filters: Filter[];
    sorts: Sort[];
    reqVariables: string[];
    ideUrl: string;
    pagination: Pagination | undefined;
}

export interface Filter {
    fieldName: string;
    matchType: string;
    constraints: Constraint[];
}

export interface Constraint {
    match: string;
    value: (string | undefined)[];
}

export interface Sort {
    field: string;
    order: string;
}

export interface Pagination {
    offset: string | undefined;
    limit: string | undefined;
}

export interface Menu {
    name: string;
    menuItems: MenuItem[];
}

export interface MenuItem {
    icon: string;
    label: string;
    url: string;
    isHref: boolean;
}

export interface Page {
    name: string;
    title: string;
    query: string | undefined;
    html: string;
    css: string;
    components: string;
    styles: string;
}

export interface Schema {
    name: string;
    type: SchemaType;
    settings: Settings;
    id: number;
    createdBy: string;
}