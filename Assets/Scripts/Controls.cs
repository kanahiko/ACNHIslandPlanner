// GENERATED AUTOMATICALLY FROM 'Assets/Prefabs/Input/Controls.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @Controls : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @Controls()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""Controls"",
    ""maps"": [
        {
            ""name"": ""MapControl"",
            ""id"": ""d591105f-e21a-4b0c-bf75-27d99ec42bb0"",
            ""actions"": [
                {
                    ""name"": ""CameraTilt"",
                    ""type"": ""Button"",
                    ""id"": ""93051b61-312c-4638-bb1c-785650fee204"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""CameraMove"",
                    ""type"": ""Value"",
                    ""id"": ""ca0a4653-eaf5-449a-9e00-3ee5c70831d5"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""ScrollButton"",
                    ""type"": ""Value"",
                    ""id"": ""b851fc6a-c2be-4815-bfea-fe078548f9c5"",
                    ""expectedControlType"": ""Digital"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""ScrollMouse"",
                    ""type"": ""Value"",
                    ""id"": ""644e747d-bd4f-4f27-a71a-bd169e8edb3a"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""SpeedChange"",
                    ""type"": ""Button"",
                    ""id"": ""0791371d-f920-402c-9eec-0222d72af05f"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""ShowGrid"",
                    ""type"": ""Button"",
                    ""id"": ""c3b733bf-2430-490c-a02f-00223a0fcb9c"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""ShowElevation"",
                    ""type"": ""Button"",
                    ""id"": ""7df2098a-5d95-4f1b-8535-39cef93b52cb"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""PlaceItem"",
                    ""type"": ""Button"",
                    ""id"": ""d6cdd37c-7532-4903-a2e3-2fe86356e15a"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""RemoveItem"",
                    ""type"": ""Button"",
                    ""id"": ""197a8b5c-747d-4918-8b59-e250ac46ec7e"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""SampleItem"",
                    ""type"": ""Button"",
                    ""id"": ""d6e6ee2c-f847-4151-9c21-1dbbcd7a2194"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""CliffConstructionTool"",
                    ""type"": ""Button"",
                    ""id"": ""05bcaacd-68b6-4dab-a9af-7044416be981"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""WaterscapingTool"",
                    ""type"": ""Button"",
                    ""id"": ""7e82695e-c294-4ba9-8f90-e5b33e6d1e6d"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""SandPermitTool"",
                    ""type"": ""Button"",
                    ""id"": ""ae2674e1-f1af-4b68-bb50-6781a30ba98f"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""PathPermitTool"",
                    ""type"": ""Button"",
                    ""id"": ""e89f957c-e33a-4428-b26f-07697cf68c2e"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""FenceTool"",
                    ""type"": ""Button"",
                    ""id"": ""8b4ef51f-4c4a-4afa-85ca-0ef3c20cf927"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""BuildingsTool"",
                    ""type"": ""Button"",
                    ""id"": ""5bd46dfe-d3c1-457c-9f5a-ff7ead375a12"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""InclineTool"",
                    ""type"": ""Button"",
                    ""id"": ""f42e8b6b-9bcb-4556-a7d0-0e3a9fbcc0b8"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""BridgeTool"",
                    ""type"": ""Button"",
                    ""id"": ""20741fef-7b27-463e-9dc5-6e702a0cc3cf"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""BushTool"",
                    ""type"": ""Button"",
                    ""id"": ""50e2bc62-4838-40dd-bfe9-516c67836dac"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""FlowersTool"",
                    ""type"": ""Button"",
                    ""id"": ""4889d753-10ed-4027-aed3-d6859afb4dc8"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""TreeTool"",
                    ""type"": ""Button"",
                    ""id"": ""800e2e29-e978-4089-b175-3fdc48cce14c"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Rotate"",
                    ""type"": ""Button"",
                    ""id"": ""47cb29f6-cc22-4f23-917a-b2e3b7e34643"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""ColorsScroll"",
                    ""type"": ""Button"",
                    ""id"": ""c5496321-d5af-48e2-9922-6da62cc8de6b"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Tips"",
                    ""type"": ""Button"",
                    ""id"": ""97ed4a05-e598-45a4-9d0b-20cc3d800f89"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""HideControls"",
                    ""type"": ""Button"",
                    ""id"": ""2c98ad01-297b-489e-a23c-36d738ebe797"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""HideMiniMap"",
                    ""type"": ""Button"",
                    ""id"": ""a1265cc8-84b1-420b-8f0d-5519460b4edc"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""e7d2c75b-460e-431d-8f33-5c9fea3cee16"",
                    ""path"": ""<Keyboard>/f"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""CameraTilt"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""WASD"",
                    ""id"": ""82a08797-aa25-43f3-8fce-204872297bd3"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""CameraMove"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""4532c585-2581-4d29-9c29-a711d6ff6fab"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""CameraMove"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""19e7c017-121f-4005-944d-f0b8020ad880"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""CameraMove"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""a507d8e7-2043-4d62-be8a-3c430a6102ca"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""CameraMove"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""43962877-7b59-4d6f-965b-6fbc4c60d590"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""CameraMove"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""Arrows"",
                    ""id"": ""9ba55818-7c9e-408d-8cdc-79c41fd35cae"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""CameraMove"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""c4fa0005-32bd-422b-8505-18bdf297a1f0"",
                    ""path"": ""<Keyboard>/upArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""CameraMove"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""b5cc7df2-ac8a-4962-9a6e-324cd96d50b2"",
                    ""path"": ""<Keyboard>/downArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""CameraMove"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""1b59fb3f-70dd-4a6c-80c4-21685a7a2ee9"",
                    ""path"": ""<Keyboard>/leftArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""CameraMove"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""ba08ebdf-a522-4f20-b4c8-b3fdab683b4e"",
                    ""path"": ""<Keyboard>/rightArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""CameraMove"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""17381a55-3546-470c-acf9-ac08253591b7"",
                    ""path"": ""<Keyboard>/shift"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""SpeedChange"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""23fe77f0-989b-426b-9939-114367228d48"",
                    ""path"": ""<Keyboard>/g"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""ShowGrid"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""19c8a60d-6461-4244-912a-357963d2f134"",
                    ""path"": ""<Keyboard>/2"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""WaterscapingTool"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""9c9e142a-936a-4c1e-a73a-0e71b64c8b1c"",
                    ""path"": ""<Keyboard>/numpad2"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""WaterscapingTool"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""31e6b714-2af9-46cd-bbed-b37c07aebf2f"",
                    ""path"": ""<Keyboard>/5"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""FenceTool"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""16b54a1f-132d-481e-a79a-90d9b53129ba"",
                    ""path"": ""<Keyboard>/numpad5"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""FenceTool"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""4f0395fa-5d19-45f0-96a7-9aacd5f85992"",
                    ""path"": ""<Keyboard>/1"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""CliffConstructionTool"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""fdefed9d-223a-40c5-80e6-03f42bde596e"",
                    ""path"": ""<Keyboard>/numpad1"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""CliffConstructionTool"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""f6f72904-a140-4ab0-8538-3833ee0d760b"",
                    ""path"": ""<Keyboard>/r"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""ShowElevation"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""ScrollPlusMinus"",
                    ""id"": ""40ab4f33-aba1-43dc-b052-6514e58173e8"",
                    ""path"": ""1DAxis"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""ScrollButton"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""ca73892c-1956-4e76-86c0-d37af5d86a4e"",
                    ""path"": ""<Keyboard>/minus"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""ScrollButton"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""cd170edd-bc7e-42d9-aeaa-0730f5cc3776"",
                    ""path"": ""<Keyboard>/equals"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""ScrollButton"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""f0c39005-e3cb-43c3-a2f4-fa5823d54800"",
                    ""path"": ""<Mouse>/scroll"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""ScrollMouse"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""cdc2ec47-5b20-4807-be38-fd59d6d10182"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""PlaceItem"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""71cbedac-cb87-4eb0-a46c-21066dd691e7"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""RemoveItem"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""93b7f9a8-0dc6-4381-9cfd-c6fa1f71eb1b"",
                    ""path"": ""<Mouse>/middleButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""SampleItem"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""de492d81-6086-411f-9c72-cd038338e77d"",
                    ""path"": ""<Keyboard>/q"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""Rotate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""a723e897-7c81-4e3d-b55b-4fa076a531e7"",
                    ""path"": ""<Keyboard>/9"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""BushTool"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""f247aa73-95ef-4899-b625-aa1e9b308ef3"",
                    ""path"": ""<Keyboard>/numpad9"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""BushTool"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""5ee8958a-4962-45f8-8a6a-e9f19a891956"",
                    ""path"": ""<Keyboard>/0"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""TreeTool"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""a3e57b6f-18c9-4d4f-95af-505cd4fdd9e3"",
                    ""path"": ""<Keyboard>/numpad0"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""TreeTool"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""45f28111-721f-49c8-b552-9589eb1d247e"",
                    ""path"": ""<Keyboard>/6"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""BuildingsTool"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""0e528dff-fe38-49db-b56a-874744bdbca3"",
                    ""path"": ""<Keyboard>/numpad6"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""BuildingsTool"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""e5015f3c-aaf2-4885-8e2a-7c9ea7ae5932"",
                    ""path"": ""<Keyboard>/e"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""ColorsScroll"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""a6e8f74d-9db8-4553-bd7f-5780bd31151d"",
                    ""path"": ""<Keyboard>/3"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""SandPermitTool"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""aa644713-7f54-4508-83b0-4f9e15395786"",
                    ""path"": ""<Keyboard>/numpad3"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""SandPermitTool"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""96a8f7db-ecc3-4ea5-993b-3322a65e492f"",
                    ""path"": ""<Keyboard>/4"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""PathPermitTool"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""36f9710b-78b5-470f-95a7-c209f04a331e"",
                    ""path"": ""<Keyboard>/numpad4"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""PathPermitTool"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""39700aea-1e15-4f81-9576-33fc5169bcc2"",
                    ""path"": ""<Keyboard>/7"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""BridgeTool"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""c04e725f-5503-47c0-8b5f-889941f5637b"",
                    ""path"": ""<Keyboard>/numpad7"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""BridgeTool"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""dafb5225-f44a-4bc9-9a0b-5e6d15923e1f"",
                    ""path"": ""<Keyboard>/8"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""InclineTool"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""0e4001d0-374a-4350-b889-5ef774a8e315"",
                    ""path"": ""<Keyboard>/numpad8"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""InclineTool"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""5479f46b-cb05-44ee-ac83-00bcb20fc9b4"",
                    ""path"": ""<Keyboard>/backquote"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""FlowersTool"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""5fcf50ca-45dc-479a-8f78-e1a52b223314"",
                    ""path"": ""<Keyboard>/t"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""Tips"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""ac80d835-bbcd-404b-91ba-d06d982dcd28"",
                    ""path"": ""<Keyboard>/h"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""HideControls"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""e6929481-f07c-43f1-b250-74afb9d96e0a"",
                    ""path"": ""<Keyboard>/m"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""HideMiniMap"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""Keyboard and Mouse"",
            ""bindingGroup"": ""Keyboard and Mouse"",
            ""devices"": [
                {
                    ""devicePath"": ""<Keyboard>"",
                    ""isOptional"": false,
                    ""isOR"": false
                },
                {
                    ""devicePath"": ""<Mouse>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        }
    ]
}");
        // MapControl
        m_MapControl = asset.FindActionMap("MapControl", throwIfNotFound: true);
        m_MapControl_CameraTilt = m_MapControl.FindAction("CameraTilt", throwIfNotFound: true);
        m_MapControl_CameraMove = m_MapControl.FindAction("CameraMove", throwIfNotFound: true);
        m_MapControl_ScrollButton = m_MapControl.FindAction("ScrollButton", throwIfNotFound: true);
        m_MapControl_ScrollMouse = m_MapControl.FindAction("ScrollMouse", throwIfNotFound: true);
        m_MapControl_SpeedChange = m_MapControl.FindAction("SpeedChange", throwIfNotFound: true);
        m_MapControl_ShowGrid = m_MapControl.FindAction("ShowGrid", throwIfNotFound: true);
        m_MapControl_ShowElevation = m_MapControl.FindAction("ShowElevation", throwIfNotFound: true);
        m_MapControl_PlaceItem = m_MapControl.FindAction("PlaceItem", throwIfNotFound: true);
        m_MapControl_RemoveItem = m_MapControl.FindAction("RemoveItem", throwIfNotFound: true);
        m_MapControl_SampleItem = m_MapControl.FindAction("SampleItem", throwIfNotFound: true);
        m_MapControl_CliffConstructionTool = m_MapControl.FindAction("CliffConstructionTool", throwIfNotFound: true);
        m_MapControl_WaterscapingTool = m_MapControl.FindAction("WaterscapingTool", throwIfNotFound: true);
        m_MapControl_SandPermitTool = m_MapControl.FindAction("SandPermitTool", throwIfNotFound: true);
        m_MapControl_PathPermitTool = m_MapControl.FindAction("PathPermitTool", throwIfNotFound: true);
        m_MapControl_FenceTool = m_MapControl.FindAction("FenceTool", throwIfNotFound: true);
        m_MapControl_BuildingsTool = m_MapControl.FindAction("BuildingsTool", throwIfNotFound: true);
        m_MapControl_InclineTool = m_MapControl.FindAction("InclineTool", throwIfNotFound: true);
        m_MapControl_BridgeTool = m_MapControl.FindAction("BridgeTool", throwIfNotFound: true);
        m_MapControl_BushTool = m_MapControl.FindAction("BushTool", throwIfNotFound: true);
        m_MapControl_FlowersTool = m_MapControl.FindAction("FlowersTool", throwIfNotFound: true);
        m_MapControl_TreeTool = m_MapControl.FindAction("TreeTool", throwIfNotFound: true);
        m_MapControl_Rotate = m_MapControl.FindAction("Rotate", throwIfNotFound: true);
        m_MapControl_ColorsScroll = m_MapControl.FindAction("ColorsScroll", throwIfNotFound: true);
        m_MapControl_Tips = m_MapControl.FindAction("Tips", throwIfNotFound: true);
        m_MapControl_HideControls = m_MapControl.FindAction("HideControls", throwIfNotFound: true);
        m_MapControl_HideMiniMap = m_MapControl.FindAction("HideMiniMap", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }

    // MapControl
    private readonly InputActionMap m_MapControl;
    private IMapControlActions m_MapControlActionsCallbackInterface;
    private readonly InputAction m_MapControl_CameraTilt;
    private readonly InputAction m_MapControl_CameraMove;
    private readonly InputAction m_MapControl_ScrollButton;
    private readonly InputAction m_MapControl_ScrollMouse;
    private readonly InputAction m_MapControl_SpeedChange;
    private readonly InputAction m_MapControl_ShowGrid;
    private readonly InputAction m_MapControl_ShowElevation;
    private readonly InputAction m_MapControl_PlaceItem;
    private readonly InputAction m_MapControl_RemoveItem;
    private readonly InputAction m_MapControl_SampleItem;
    private readonly InputAction m_MapControl_CliffConstructionTool;
    private readonly InputAction m_MapControl_WaterscapingTool;
    private readonly InputAction m_MapControl_SandPermitTool;
    private readonly InputAction m_MapControl_PathPermitTool;
    private readonly InputAction m_MapControl_FenceTool;
    private readonly InputAction m_MapControl_BuildingsTool;
    private readonly InputAction m_MapControl_InclineTool;
    private readonly InputAction m_MapControl_BridgeTool;
    private readonly InputAction m_MapControl_BushTool;
    private readonly InputAction m_MapControl_FlowersTool;
    private readonly InputAction m_MapControl_TreeTool;
    private readonly InputAction m_MapControl_Rotate;
    private readonly InputAction m_MapControl_ColorsScroll;
    private readonly InputAction m_MapControl_Tips;
    private readonly InputAction m_MapControl_HideControls;
    private readonly InputAction m_MapControl_HideMiniMap;
    public struct MapControlActions
    {
        private @Controls m_Wrapper;
        public MapControlActions(@Controls wrapper) { m_Wrapper = wrapper; }
        public InputAction @CameraTilt => m_Wrapper.m_MapControl_CameraTilt;
        public InputAction @CameraMove => m_Wrapper.m_MapControl_CameraMove;
        public InputAction @ScrollButton => m_Wrapper.m_MapControl_ScrollButton;
        public InputAction @ScrollMouse => m_Wrapper.m_MapControl_ScrollMouse;
        public InputAction @SpeedChange => m_Wrapper.m_MapControl_SpeedChange;
        public InputAction @ShowGrid => m_Wrapper.m_MapControl_ShowGrid;
        public InputAction @ShowElevation => m_Wrapper.m_MapControl_ShowElevation;
        public InputAction @PlaceItem => m_Wrapper.m_MapControl_PlaceItem;
        public InputAction @RemoveItem => m_Wrapper.m_MapControl_RemoveItem;
        public InputAction @SampleItem => m_Wrapper.m_MapControl_SampleItem;
        public InputAction @CliffConstructionTool => m_Wrapper.m_MapControl_CliffConstructionTool;
        public InputAction @WaterscapingTool => m_Wrapper.m_MapControl_WaterscapingTool;
        public InputAction @SandPermitTool => m_Wrapper.m_MapControl_SandPermitTool;
        public InputAction @PathPermitTool => m_Wrapper.m_MapControl_PathPermitTool;
        public InputAction @FenceTool => m_Wrapper.m_MapControl_FenceTool;
        public InputAction @BuildingsTool => m_Wrapper.m_MapControl_BuildingsTool;
        public InputAction @InclineTool => m_Wrapper.m_MapControl_InclineTool;
        public InputAction @BridgeTool => m_Wrapper.m_MapControl_BridgeTool;
        public InputAction @BushTool => m_Wrapper.m_MapControl_BushTool;
        public InputAction @FlowersTool => m_Wrapper.m_MapControl_FlowersTool;
        public InputAction @TreeTool => m_Wrapper.m_MapControl_TreeTool;
        public InputAction @Rotate => m_Wrapper.m_MapControl_Rotate;
        public InputAction @ColorsScroll => m_Wrapper.m_MapControl_ColorsScroll;
        public InputAction @Tips => m_Wrapper.m_MapControl_Tips;
        public InputAction @HideControls => m_Wrapper.m_MapControl_HideControls;
        public InputAction @HideMiniMap => m_Wrapper.m_MapControl_HideMiniMap;
        public InputActionMap Get() { return m_Wrapper.m_MapControl; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(MapControlActions set) { return set.Get(); }
        public void SetCallbacks(IMapControlActions instance)
        {
            if (m_Wrapper.m_MapControlActionsCallbackInterface != null)
            {
                @CameraTilt.started -= m_Wrapper.m_MapControlActionsCallbackInterface.OnCameraTilt;
                @CameraTilt.performed -= m_Wrapper.m_MapControlActionsCallbackInterface.OnCameraTilt;
                @CameraTilt.canceled -= m_Wrapper.m_MapControlActionsCallbackInterface.OnCameraTilt;
                @CameraMove.started -= m_Wrapper.m_MapControlActionsCallbackInterface.OnCameraMove;
                @CameraMove.performed -= m_Wrapper.m_MapControlActionsCallbackInterface.OnCameraMove;
                @CameraMove.canceled -= m_Wrapper.m_MapControlActionsCallbackInterface.OnCameraMove;
                @ScrollButton.started -= m_Wrapper.m_MapControlActionsCallbackInterface.OnScrollButton;
                @ScrollButton.performed -= m_Wrapper.m_MapControlActionsCallbackInterface.OnScrollButton;
                @ScrollButton.canceled -= m_Wrapper.m_MapControlActionsCallbackInterface.OnScrollButton;
                @ScrollMouse.started -= m_Wrapper.m_MapControlActionsCallbackInterface.OnScrollMouse;
                @ScrollMouse.performed -= m_Wrapper.m_MapControlActionsCallbackInterface.OnScrollMouse;
                @ScrollMouse.canceled -= m_Wrapper.m_MapControlActionsCallbackInterface.OnScrollMouse;
                @SpeedChange.started -= m_Wrapper.m_MapControlActionsCallbackInterface.OnSpeedChange;
                @SpeedChange.performed -= m_Wrapper.m_MapControlActionsCallbackInterface.OnSpeedChange;
                @SpeedChange.canceled -= m_Wrapper.m_MapControlActionsCallbackInterface.OnSpeedChange;
                @ShowGrid.started -= m_Wrapper.m_MapControlActionsCallbackInterface.OnShowGrid;
                @ShowGrid.performed -= m_Wrapper.m_MapControlActionsCallbackInterface.OnShowGrid;
                @ShowGrid.canceled -= m_Wrapper.m_MapControlActionsCallbackInterface.OnShowGrid;
                @ShowElevation.started -= m_Wrapper.m_MapControlActionsCallbackInterface.OnShowElevation;
                @ShowElevation.performed -= m_Wrapper.m_MapControlActionsCallbackInterface.OnShowElevation;
                @ShowElevation.canceled -= m_Wrapper.m_MapControlActionsCallbackInterface.OnShowElevation;
                @PlaceItem.started -= m_Wrapper.m_MapControlActionsCallbackInterface.OnPlaceItem;
                @PlaceItem.performed -= m_Wrapper.m_MapControlActionsCallbackInterface.OnPlaceItem;
                @PlaceItem.canceled -= m_Wrapper.m_MapControlActionsCallbackInterface.OnPlaceItem;
                @RemoveItem.started -= m_Wrapper.m_MapControlActionsCallbackInterface.OnRemoveItem;
                @RemoveItem.performed -= m_Wrapper.m_MapControlActionsCallbackInterface.OnRemoveItem;
                @RemoveItem.canceled -= m_Wrapper.m_MapControlActionsCallbackInterface.OnRemoveItem;
                @SampleItem.started -= m_Wrapper.m_MapControlActionsCallbackInterface.OnSampleItem;
                @SampleItem.performed -= m_Wrapper.m_MapControlActionsCallbackInterface.OnSampleItem;
                @SampleItem.canceled -= m_Wrapper.m_MapControlActionsCallbackInterface.OnSampleItem;
                @CliffConstructionTool.started -= m_Wrapper.m_MapControlActionsCallbackInterface.OnCliffConstructionTool;
                @CliffConstructionTool.performed -= m_Wrapper.m_MapControlActionsCallbackInterface.OnCliffConstructionTool;
                @CliffConstructionTool.canceled -= m_Wrapper.m_MapControlActionsCallbackInterface.OnCliffConstructionTool;
                @WaterscapingTool.started -= m_Wrapper.m_MapControlActionsCallbackInterface.OnWaterscapingTool;
                @WaterscapingTool.performed -= m_Wrapper.m_MapControlActionsCallbackInterface.OnWaterscapingTool;
                @WaterscapingTool.canceled -= m_Wrapper.m_MapControlActionsCallbackInterface.OnWaterscapingTool;
                @SandPermitTool.started -= m_Wrapper.m_MapControlActionsCallbackInterface.OnSandPermitTool;
                @SandPermitTool.performed -= m_Wrapper.m_MapControlActionsCallbackInterface.OnSandPermitTool;
                @SandPermitTool.canceled -= m_Wrapper.m_MapControlActionsCallbackInterface.OnSandPermitTool;
                @PathPermitTool.started -= m_Wrapper.m_MapControlActionsCallbackInterface.OnPathPermitTool;
                @PathPermitTool.performed -= m_Wrapper.m_MapControlActionsCallbackInterface.OnPathPermitTool;
                @PathPermitTool.canceled -= m_Wrapper.m_MapControlActionsCallbackInterface.OnPathPermitTool;
                @FenceTool.started -= m_Wrapper.m_MapControlActionsCallbackInterface.OnFenceTool;
                @FenceTool.performed -= m_Wrapper.m_MapControlActionsCallbackInterface.OnFenceTool;
                @FenceTool.canceled -= m_Wrapper.m_MapControlActionsCallbackInterface.OnFenceTool;
                @BuildingsTool.started -= m_Wrapper.m_MapControlActionsCallbackInterface.OnBuildingsTool;
                @BuildingsTool.performed -= m_Wrapper.m_MapControlActionsCallbackInterface.OnBuildingsTool;
                @BuildingsTool.canceled -= m_Wrapper.m_MapControlActionsCallbackInterface.OnBuildingsTool;
                @InclineTool.started -= m_Wrapper.m_MapControlActionsCallbackInterface.OnInclineTool;
                @InclineTool.performed -= m_Wrapper.m_MapControlActionsCallbackInterface.OnInclineTool;
                @InclineTool.canceled -= m_Wrapper.m_MapControlActionsCallbackInterface.OnInclineTool;
                @BridgeTool.started -= m_Wrapper.m_MapControlActionsCallbackInterface.OnBridgeTool;
                @BridgeTool.performed -= m_Wrapper.m_MapControlActionsCallbackInterface.OnBridgeTool;
                @BridgeTool.canceled -= m_Wrapper.m_MapControlActionsCallbackInterface.OnBridgeTool;
                @BushTool.started -= m_Wrapper.m_MapControlActionsCallbackInterface.OnBushTool;
                @BushTool.performed -= m_Wrapper.m_MapControlActionsCallbackInterface.OnBushTool;
                @BushTool.canceled -= m_Wrapper.m_MapControlActionsCallbackInterface.OnBushTool;
                @FlowersTool.started -= m_Wrapper.m_MapControlActionsCallbackInterface.OnFlowersTool;
                @FlowersTool.performed -= m_Wrapper.m_MapControlActionsCallbackInterface.OnFlowersTool;
                @FlowersTool.canceled -= m_Wrapper.m_MapControlActionsCallbackInterface.OnFlowersTool;
                @TreeTool.started -= m_Wrapper.m_MapControlActionsCallbackInterface.OnTreeTool;
                @TreeTool.performed -= m_Wrapper.m_MapControlActionsCallbackInterface.OnTreeTool;
                @TreeTool.canceled -= m_Wrapper.m_MapControlActionsCallbackInterface.OnTreeTool;
                @Rotate.started -= m_Wrapper.m_MapControlActionsCallbackInterface.OnRotate;
                @Rotate.performed -= m_Wrapper.m_MapControlActionsCallbackInterface.OnRotate;
                @Rotate.canceled -= m_Wrapper.m_MapControlActionsCallbackInterface.OnRotate;
                @ColorsScroll.started -= m_Wrapper.m_MapControlActionsCallbackInterface.OnColorsScroll;
                @ColorsScroll.performed -= m_Wrapper.m_MapControlActionsCallbackInterface.OnColorsScroll;
                @ColorsScroll.canceled -= m_Wrapper.m_MapControlActionsCallbackInterface.OnColorsScroll;
                @Tips.started -= m_Wrapper.m_MapControlActionsCallbackInterface.OnTips;
                @Tips.performed -= m_Wrapper.m_MapControlActionsCallbackInterface.OnTips;
                @Tips.canceled -= m_Wrapper.m_MapControlActionsCallbackInterface.OnTips;
                @HideControls.started -= m_Wrapper.m_MapControlActionsCallbackInterface.OnHideControls;
                @HideControls.performed -= m_Wrapper.m_MapControlActionsCallbackInterface.OnHideControls;
                @HideControls.canceled -= m_Wrapper.m_MapControlActionsCallbackInterface.OnHideControls;
                @HideMiniMap.started -= m_Wrapper.m_MapControlActionsCallbackInterface.OnHideMiniMap;
                @HideMiniMap.performed -= m_Wrapper.m_MapControlActionsCallbackInterface.OnHideMiniMap;
                @HideMiniMap.canceled -= m_Wrapper.m_MapControlActionsCallbackInterface.OnHideMiniMap;
            }
            m_Wrapper.m_MapControlActionsCallbackInterface = instance;
            if (instance != null)
            {
                @CameraTilt.started += instance.OnCameraTilt;
                @CameraTilt.performed += instance.OnCameraTilt;
                @CameraTilt.canceled += instance.OnCameraTilt;
                @CameraMove.started += instance.OnCameraMove;
                @CameraMove.performed += instance.OnCameraMove;
                @CameraMove.canceled += instance.OnCameraMove;
                @ScrollButton.started += instance.OnScrollButton;
                @ScrollButton.performed += instance.OnScrollButton;
                @ScrollButton.canceled += instance.OnScrollButton;
                @ScrollMouse.started += instance.OnScrollMouse;
                @ScrollMouse.performed += instance.OnScrollMouse;
                @ScrollMouse.canceled += instance.OnScrollMouse;
                @SpeedChange.started += instance.OnSpeedChange;
                @SpeedChange.performed += instance.OnSpeedChange;
                @SpeedChange.canceled += instance.OnSpeedChange;
                @ShowGrid.started += instance.OnShowGrid;
                @ShowGrid.performed += instance.OnShowGrid;
                @ShowGrid.canceled += instance.OnShowGrid;
                @ShowElevation.started += instance.OnShowElevation;
                @ShowElevation.performed += instance.OnShowElevation;
                @ShowElevation.canceled += instance.OnShowElevation;
                @PlaceItem.started += instance.OnPlaceItem;
                @PlaceItem.performed += instance.OnPlaceItem;
                @PlaceItem.canceled += instance.OnPlaceItem;
                @RemoveItem.started += instance.OnRemoveItem;
                @RemoveItem.performed += instance.OnRemoveItem;
                @RemoveItem.canceled += instance.OnRemoveItem;
                @SampleItem.started += instance.OnSampleItem;
                @SampleItem.performed += instance.OnSampleItem;
                @SampleItem.canceled += instance.OnSampleItem;
                @CliffConstructionTool.started += instance.OnCliffConstructionTool;
                @CliffConstructionTool.performed += instance.OnCliffConstructionTool;
                @CliffConstructionTool.canceled += instance.OnCliffConstructionTool;
                @WaterscapingTool.started += instance.OnWaterscapingTool;
                @WaterscapingTool.performed += instance.OnWaterscapingTool;
                @WaterscapingTool.canceled += instance.OnWaterscapingTool;
                @SandPermitTool.started += instance.OnSandPermitTool;
                @SandPermitTool.performed += instance.OnSandPermitTool;
                @SandPermitTool.canceled += instance.OnSandPermitTool;
                @PathPermitTool.started += instance.OnPathPermitTool;
                @PathPermitTool.performed += instance.OnPathPermitTool;
                @PathPermitTool.canceled += instance.OnPathPermitTool;
                @FenceTool.started += instance.OnFenceTool;
                @FenceTool.performed += instance.OnFenceTool;
                @FenceTool.canceled += instance.OnFenceTool;
                @BuildingsTool.started += instance.OnBuildingsTool;
                @BuildingsTool.performed += instance.OnBuildingsTool;
                @BuildingsTool.canceled += instance.OnBuildingsTool;
                @InclineTool.started += instance.OnInclineTool;
                @InclineTool.performed += instance.OnInclineTool;
                @InclineTool.canceled += instance.OnInclineTool;
                @BridgeTool.started += instance.OnBridgeTool;
                @BridgeTool.performed += instance.OnBridgeTool;
                @BridgeTool.canceled += instance.OnBridgeTool;
                @BushTool.started += instance.OnBushTool;
                @BushTool.performed += instance.OnBushTool;
                @BushTool.canceled += instance.OnBushTool;
                @FlowersTool.started += instance.OnFlowersTool;
                @FlowersTool.performed += instance.OnFlowersTool;
                @FlowersTool.canceled += instance.OnFlowersTool;
                @TreeTool.started += instance.OnTreeTool;
                @TreeTool.performed += instance.OnTreeTool;
                @TreeTool.canceled += instance.OnTreeTool;
                @Rotate.started += instance.OnRotate;
                @Rotate.performed += instance.OnRotate;
                @Rotate.canceled += instance.OnRotate;
                @ColorsScroll.started += instance.OnColorsScroll;
                @ColorsScroll.performed += instance.OnColorsScroll;
                @ColorsScroll.canceled += instance.OnColorsScroll;
                @Tips.started += instance.OnTips;
                @Tips.performed += instance.OnTips;
                @Tips.canceled += instance.OnTips;
                @HideControls.started += instance.OnHideControls;
                @HideControls.performed += instance.OnHideControls;
                @HideControls.canceled += instance.OnHideControls;
                @HideMiniMap.started += instance.OnHideMiniMap;
                @HideMiniMap.performed += instance.OnHideMiniMap;
                @HideMiniMap.canceled += instance.OnHideMiniMap;
            }
        }
    }
    public MapControlActions @MapControl => new MapControlActions(this);
    private int m_KeyboardandMouseSchemeIndex = -1;
    public InputControlScheme KeyboardandMouseScheme
    {
        get
        {
            if (m_KeyboardandMouseSchemeIndex == -1) m_KeyboardandMouseSchemeIndex = asset.FindControlSchemeIndex("Keyboard and Mouse");
            return asset.controlSchemes[m_KeyboardandMouseSchemeIndex];
        }
    }
    public interface IMapControlActions
    {
        void OnCameraTilt(InputAction.CallbackContext context);
        void OnCameraMove(InputAction.CallbackContext context);
        void OnScrollButton(InputAction.CallbackContext context);
        void OnScrollMouse(InputAction.CallbackContext context);
        void OnSpeedChange(InputAction.CallbackContext context);
        void OnShowGrid(InputAction.CallbackContext context);
        void OnShowElevation(InputAction.CallbackContext context);
        void OnPlaceItem(InputAction.CallbackContext context);
        void OnRemoveItem(InputAction.CallbackContext context);
        void OnSampleItem(InputAction.CallbackContext context);
        void OnCliffConstructionTool(InputAction.CallbackContext context);
        void OnWaterscapingTool(InputAction.CallbackContext context);
        void OnSandPermitTool(InputAction.CallbackContext context);
        void OnPathPermitTool(InputAction.CallbackContext context);
        void OnFenceTool(InputAction.CallbackContext context);
        void OnBuildingsTool(InputAction.CallbackContext context);
        void OnInclineTool(InputAction.CallbackContext context);
        void OnBridgeTool(InputAction.CallbackContext context);
        void OnBushTool(InputAction.CallbackContext context);
        void OnFlowersTool(InputAction.CallbackContext context);
        void OnTreeTool(InputAction.CallbackContext context);
        void OnRotate(InputAction.CallbackContext context);
        void OnColorsScroll(InputAction.CallbackContext context);
        void OnTips(InputAction.CallbackContext context);
        void OnHideControls(InputAction.CallbackContext context);
        void OnHideMiniMap(InputAction.CallbackContext context);
    }
}
