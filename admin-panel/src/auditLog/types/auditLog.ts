//----------------------
// <auto-generated>
//     Generated using the NSwag toolchain v11.1.0.0 (Newtonsoft.Json v13.0.0.0) (http://NJsonSchema.org)
// </auto-generated>
//----------------------







export enum ActionType {
    Create = "create",
    Update = "update",
    Delete = "delete",
}

export interface AuditLog {
    id: number;
    userId: string;
    userName: string;
    action: ActionType;
    entityName: string;
    recordId: string;
    payload: { [key: string]: any; };
    createdAt: Date;
}