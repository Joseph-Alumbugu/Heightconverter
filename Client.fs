namespace Project23

open WebSharper
open WebSharper.JavaScript
open WebSharper.UI
open WebSharper.UI.Client
open WebSharper.UI.Templating
open WebSharper.Sitelets
open WebSharper.Forms
open WebSharper.UI.Html

type IndexTemplate = Template<"wwwroot/index.html", ClientLoad.FromDocument>

// Our SPA endpoints
type EndPoint =
    | [<EndPoint "/">] Home
    | [<EndPoint "/form">] Form

[<JavaScript>]
module Pages =
    

    

    let FormPage() = 
           
                  Form.Return (fun name ux options message pp ->
                    name, ux, options, message, pp)
                        <*> (Form.Yield "" |> Validation.IsNotEmpty "Can't be empty")
                        <*> Form.Yield ""
                        <*> Form.Yield ""
                        <*> Form.Yield ""
                        <*> Form.Yield false
                        |> Form.WithSubmit
                        |> Form.Run (fun ((name, ux, options, message, pp) as data) ->
                            JS.Alert $"You submitted: {data}"
                        )
                        |> Form.Render (fun name ux options message pp submitter ->
                            IndexTemplate.Form1()
                                .Title("Thank you for leaving a feedback")              
                                .Name(name)
                                .ux(ux)
                                .Message(message)
                                .PrivacyPolicy(pp)
                                .OnSubmit(fun e -> submitter.Trigger())
                                .Doc()
                   )


    let HomePage() =
           
                // Create a variable to store the input value
                let input = Var.Create "0"

                // Create a view for the input value
                let viewInput = input.View

                // Define a function to convert Gram to Milligram
                let FeetstoCentiMeter() =
                    View.Map (fun (value:string) ->
                       if (value <> "0") then
                            let result = float value * 30.48
                    
                            $"{result}"
                       else
                            "Error"
                    ) viewInput

                // Define a function to convert Gram to Kilogram
                let FeetstoMeters() = 
                    View.Map ( fun (value:string) ->
                        if (value <> "0") then  
                            let result = float value * 0.3048
                            $"{result}"
                        else
                            "Error" 
                    ) viewInput
        
                // Render the conversion page using the MassTemplate
                IndexTemplate.conversionPage()
                    .ToHomepage("/#/form")                                                                               
                    .input(input)
                    .convertFromResult(viewInput)
                    .convertToResult1(FeetstoCentiMeter())
                    .convertToResult2(FeetstoMeters())                   
                    .Doc()


  
 
[<JavaScript>]
module App =
    open WebSharper.UI.Notation

    // Create a router for our endpoints
    let router = Router.Infer<EndPoint>()
    // Install our client-side router and track the current page
    let currentPage = Router.InstallHash Home router

    type Router<'T when 'T: equality> with
        member this.LinkHash (ep: 'T) = "#" + this.Link ep

    [<SPAEntryPoint>]
    let Main () =
        let renderInnerPage (currentPage: Var<EndPoint>) =
            currentPage.View.Map (fun endpoint ->
                match endpoint with
                | Home      -> Pages.HomePage()
                | Form      -> Pages.FormPage()
            )
            |> Doc.EmbedView
        
        IndexTemplate()
            .Url_Home(router.LinkHash EndPoint.Home)
            .Url_Page1(router.LinkHash EndPoint.Form)           
            .MainContainer(renderInnerPage currentPage)
            .Bind()