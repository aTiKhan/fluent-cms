# Fluent CMS - CRUD (Create, Read, Update, Delete) for any entities
[![GitHub stars](https://img.shields.io/github/stars/fluent-cms/fluent-cms.svg?style=social&label=Star)](https://github.com/fluent-cms/fluent-cms/stargazers)
Welcome to Fluent CMS! If you find it useful, please give it a star ⭐

## What is it

Fluent CMS is an open-source content management system designed to simplify the workflow for web development. 
- It provides a set of CRUD(Create, Read, Update, Delete) Restful APIs for any entities based on your configuration.
- You can add your own logic by registering hook functions before or after access databases.
- It provides admin panel for you to manage data, the admin panel have a rich set of inputs, text, number, datetime, dropdown, image, image gallery, and rich text editor.
- It provides a schema builder for you to define entity and attributes.
  
## Live Demo - A blog website build by Fluent CMS 
   source code [FluentCMS.Blog](server%2FFluentCMS.Blog)
   - Admin Panel https://fluent-cms-admin.azurewebsites.net/
      - Email: `admin@cms.com`
      - Password: `Admin1!`  
   - Public Site : https://fluent-cms-ui.azurewebsites.net/
    
## Add Fluent CMS to your own project
1. Create your own WebApplication.
2. Add FluentCMS package
   ```shell
   dotnet add package FluentCMS
   # the next command copy compiled Admin Panel code to your wwwroot, 
   # The frontend code was write in React and Jquery, source code is admin-ui, also in this repo
   # The following command is for Mac,
   # for windows the directory should be at $(NuGetPackageRoot)fluentcms\1.0.0\staticwebassets
   # Please change 0.0.3 to the correct version number    
   cp -a ~/.nuget/packages/fluentcms/0.0.3/staticwebassets wwwroot 
   ```
3. Modify Program.cs, add the following line before builder.Build(), the input parameter is the connection string of database.
   ```
   //Currently FluentCMS support AddSqliteCms, AddSqlServerCms 
   builder.AddSqliteCms("Data Source=cms.db").PrintVersion();
   var app = builder.Build();
   ```
4. Add the following line After builder.Build()
   ```
   //this function bootstrap router, initialize Fluent CMS schema table
   await app.UseCmsAsync(false);
   ```
5. If everthing is good, when the app starts, when you go to the home page, you should see the empty Admin Panel
   Here is a quickstart on how to use the Admin Panel [Quickstart.md](doc%2FQuickstart.md) 
6. If you want to have a look at how FluentCMS handles one to many, many-to-many relationships, just add the following code
    ```
    var schemaService = app.GetCmsSchemaService();
    await schemaService.AddOrSaveSimpleEntity("student", "Name", null, null);
    await schemaService.AddOrSaveSimpleEntity("teacher", "Name", null, null);
    await schemaService.AddOrSaveSimpleEntity("class", "Name", "teacher", "student");   
   ```
   These code created 3 entity, class and teacher has many-to-one relationship. class and student has many-to-many relationship
7. To Add you own business logic, you can add hook, before or after CRUD. For more hook example, have a look at  [Program.cs](server%2FFluentCMS.App%2FProgram.cs)
    ```
   var hooks = app.GetCmsHookFactory();
   hooks.AddHook("teacher", Occasion.BeforeInsert, Next.Continue, (IDictionary<string,object> payload) =>
   {
      payload["Name"] = "Teacher " + payload["Name"];
    });
   ```
8. Source code of this example can be found at  [WebApiExamples](examples%2FWebApiExamples)  
## Core Concepts
   - Understanding concepts like Entity, Attributes, View is crucial for using and customizing Fluent CMS.     
   - Detailed in [Concepts.md](doc%2FConcepts.md)
## Development
![overview.png](doc%2Fdiagrams%2Foverview.png)
- Web Server: 
  - Code [FluentCMS](..%2Fserver%2FFluentCMS)
  - Doc [Server](doc%2FDevelopment.md#Server )
- Admin Panel Client:
  - Code [admin-ui](..%2Fadmin-ui)
  - Doc [Admin-Panel-UI](doc%2FDevelopment.md#Admin-Panel-UI)
- Schema Builder: 
  - Code [schema-ui](..%2Fserver%2FFluentCMS%2Fwwwroot%2Fschema-ui)
  - Doc [Schema-Builder-UI](doc%2FDevelopment.md#Schema-Builder-UI)
- Demo Publish Site:
  - Code [ui](..%2Fui)
