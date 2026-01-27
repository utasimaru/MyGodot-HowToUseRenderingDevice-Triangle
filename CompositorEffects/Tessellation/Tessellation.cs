using Godot;
using System;
using System.Runtime.InteropServices;

[Tool, GlobalClass]
public partial class Tessellation : CompositorEffect
{
    RenderingDevice rd;
    Rid pipeline;
    Rid shader;
    Rid vertex_position_buffer;
    Rid vertex_array;
    Godot.Color[] clear_colors;
    Rid image_texture;
    Rid depth_texture;
    Rid screen_buffer;
    public Tessellation()
    {
        EffectCallbackType = EffectCallbackTypeEnum.PostTransparent;
    }
    public void _initialize_rendering(RenderSceneBuffersRD renderscene_buffers)
    {
        rd = RenderingServer.GetRenderingDevice();

        shader = _compile_shader();
        //Creates a new vertex attribute for position data.
        var vertex_attribute_position = new RDVertexAttribute();
        //The format is set to 32-bit float with 2 components (x, y).
        vertex_attribute_position.Format = RenderingDevice.DataFormat.R32G32Sfloat;
        //The attribute is set to be used per vertex.
        vertex_attribute_position.Frequency = RenderingDevice.VertexFrequency.Vertex;
        //The location of the attribute in the shader is set to 0.
        vertex_attribute_position.Location = 0;
        //The offset within the data is set to 0 bytes.
        vertex_attribute_position.Offset = 0;
        //The stride (size of each vertex attribute) is set to 8 bytes (4 bytes per component * 2 components).
        vertex_attribute_position.Stride = 4 * 2;


        //This format will be used to define the structure of vertex data in a rendering pipeline.
        var vertex_format = rd.VertexFormatCreate([vertex_attribute_position]);

        //Create a vertex buffer for positions
        ReadOnlySpan<float> vertices = 
            [0.0f, 0.5f, 
            -0.5f, -0.5f, 
            0.0f, -0.8f,
            0.5f, -0.5f];
        var vertices_position_packed = MemoryMarshal.AsBytes(vertices);
        vertex_position_buffer = rd.VertexBufferCreate((uint)vertices_position_packed.Length, vertices_position_packed, false);


        vertex_array = rd.VertexArrayCreate(4, vertex_format, [vertex_position_buffer]);

        // Initializes a new rasterization state for the rendering pipeline.
        var rasterization_state = new RDPipelineRasterizationState();
        rasterization_state.PatchControlPoints = 4;
        // The polygon mode is set to fill, meaning polygons will be filled with color.
        rasterization_state.Wireframe = true;
        // The cull mode is set to disabled, meaning no polygons will be culled.
        rasterization_state.CullMode = RenderingDevice.PolygonCullMode.Disabled;
        // Depth clamping is disabled, meaning depth values will not be clamped.
        rasterization_state.EnableDepthClamp = false;
        // Line width is set to 1.0.
        rasterization_state.LineWidth = 2.0f;
        //The front face of polygons is set to be clockwise.
        rasterization_state.FrontFace = RenderingDevice.PolygonFrontFace.Clockwise;
        // Depth bias is disabled, meaning no depth bias will be applied.
        rasterization_state.DepthBiasEnabled = false;

        // Initializes a new multisample state for the rendering pipeline.
        var multisample_state = new RDPipelineMultisampleState();
        // Multisampling is disabled
        multisample_state.EnableSampleShading = false;

        multisample_state.SampleCount = RenderingDevice.TextureSamples.Samples1;

        multisample_state.MinSampleShading = 1.0f;

        // Initializes a depth-stencil state for the rendering pipeline.
        // The depth test is disabled.
        var stencil_state = new RDPipelineDepthStencilState();

        stencil_state.EnableDepthTest = false;

        // Initializes a color blend state for the rendering pipeline.
        var color_blend_state = new RDPipelineColorBlendState();
        // Initializes a color blend state attachment for first and only color attachment.
        // The resulting color is the new color.
        var color_attachment = new RDPipelineColorBlendStateAttachment();

        color_attachment.EnableBlend = true;
        color_attachment.WriteA = true;
        color_attachment.WriteB = true;
        color_attachment.WriteG = true;
        color_attachment.WriteR = true;
        color_attachment.AlphaBlendOp = RenderingDevice.BlendOperation.Add;
        color_attachment.ColorBlendOp = RenderingDevice.BlendOperation.Add;
        color_attachment.SrcColorBlendFactor = RenderingDevice.BlendFactor.One;
        color_attachment.DstColorBlendFactor = RenderingDevice.BlendFactor.Zero;
        color_attachment.SrcAlphaBlendFactor = RenderingDevice.BlendFactor.One;
        color_attachment.DstAlphaBlendFactor = RenderingDevice.BlendFactor.Zero;
        color_blend_state.Attachments.Add(color_attachment);
        color_blend_state.EnableLogicOp = false;
        color_blend_state.LogicOp = RenderingDevice.LogicOperation.Copy;

        // Initialize the frame buffer for rendering
        image_texture = renderscene_buffers.GetColorTexture();
        depth_texture = renderscene_buffers.GetDepthTexture();
        screen_buffer = rd.FramebufferCreate([image_texture, depth_texture]);
        var fb_format = rd.FramebufferGetFormat(screen_buffer);

        pipeline = rd.RenderPipelineCreate(
            shader, fb_format, vertex_format, RenderingDevice.RenderPrimitive.TesselationPatch,
            rasterization_state, multisample_state, stencil_state, color_blend_state,
            0, 0, []
        );

        clear_colors = [new Godot.Color(0.2f, 0.2f, 0.2f, 1.0f)];
    }
    public override void _Notification(int what)
    {
        base._Notification(what);
        switch ((long)what)
        {
            case NotificationPredelete:
                RenderingServer.CallOnRenderThread(new Callable(this, MethodName.FreeRids));
                break;
        }
    }
    public void FreeRids()
    {
        if (rd == null) return;

        if (shader.IsValid) rd.FreeRid(shader);

        if (vertex_array.IsValid)
            rd.FreeRid(vertex_array);

        if (vertex_position_buffer.IsValid)
            rd.FreeRid(vertex_position_buffer);

        if (rd.FramebufferIsValid(screen_buffer))
            rd.FreeRid(screen_buffer);
    }
    public override void _RenderCallback(int effectCallbackType, RenderData renderData)
    {
        base._RenderCallback(effectCallbackType, renderData);
        if (effectCallbackType == ((int)EffectCallbackTypeEnum.PostTransparent))
        {
            RenderSceneBuffersRD render_scene_buffers = (RenderSceneBuffersRD)renderData.GetRenderSceneBuffers();


            if (render_scene_buffers != null)
            {
                var size = render_scene_buffers.GetInternalSize();
                if (size.X == 0 || size.Y == 0) return;

                if (rd == null) _initialize_rendering(render_scene_buffers);

                else
                {
                    var new_image_texture = render_scene_buffers.GetColorTexture();

                    var new_depth_texture = render_scene_buffers.GetDepthTexture();
                    if (new_image_texture != image_texture || new_depth_texture != depth_texture)
                    {
                        image_texture = new_image_texture;
                        depth_texture = new_depth_texture;
                        if (rd.FramebufferIsValid(screen_buffer)) rd.FreeRid(screen_buffer);
                        // Creates a framebuffer object (FBO) that encapsulates the color and depth textures.
                        // The FBO is used as the target for rendering operations
                        screen_buffer = rd.FramebufferCreate([image_texture, depth_texture]);
                    }
                }

                rd.DrawCommandBeginLabel("Draw a simple triangle", new Godot.Color(1.0f, 1.0f, 1.0f, 1.0f));

                var draw_list = rd.DrawListBegin(screen_buffer, RenderingDevice.DrawFlags.IgnoreAll, clear_colors);
                rd.DrawListBindRenderPipeline(draw_list, pipeline);
                rd.DrawListBindVertexArray(draw_list, vertex_array);
                rd.DrawListDraw(draw_list, false, 1, 0);
                rd.DrawListEnd();
                rd.DrawCommandEndLabel();
            }
        }
    }


    Rid _compile_shader()
    {
        var src = new RDShaderSource();

        src.SourceVertex = _source_vertex;
        src.SourceFragment = _source_fragment;

        src.SourceTesselationControl = _source_tcs;
        src.SourceTesselationEvaluation = _source_tes;

        RDShaderSpirV shader_spirv = rd.ShaderCompileSpirVFromSource(src);

        var err = shader_spirv.CompileErrorFragment;
        if (err != "") GD.Print("fragment shader で エラー:" + err);
        err = shader_spirv.CompileErrorVertex;
        if (err != "") GD.Print("vertex shader で エラー:" + err);

        err = shader_spirv.CompileErrorTesselationControl;
        if (err != "") GD.Print("tcs で エラー:" + err);
        err = shader_spirv.CompileErrorTesselationEvaluation;
        if (err != "") GD.Print("tes で エラー:" + err);

        Rid p_shader = rd.ShaderCreateFromSpirV(shader_spirv);
        return p_shader;
    }
    const string _source_vertex = @"
		#version 450
		
		layout(location = 0) in vec2 inPosition;

		layout(location = 0) out vec2 outPosition;

		void main() {
			outPosition = inPosition;
		}
		";
    const string _source_tcs = @"
		#version 450

        layout(vertices = 1) out;
        layout(location = 0) in vec2 vertices[];

        layout(location = 0) patch out vec2 out_vertices[4];

        layout(location = 4) patch out Cache {
            vec2 A;
            vec2 B;
        } cache;


        void main() {
            out_vertices[0] = vertices[0];
            out_vertices[1] = vertices[1];
            out_vertices[2] = vertices[2];
            out_vertices[3] = vertices[3];

            vec2 s = vertices[1];
            vec2 h = vertices[2];
            vec2 e = vertices[3];
            vec2 A = 2.0 * (s+e-h-h);
            cache.A = A;
            cache.B = A + s - e;

            float level = 8.0;
            gl_TessLevelOuter[0] = 4.0;
            gl_TessLevelOuter[1] = level;
            gl_TessLevelOuter[2] = 4.0;

            gl_TessLevelInner[0] = 3;
        }
		";
    const string _source_tes = @"
		#version 450

        layout(triangles, fractional_even_spacing, ccw) in;

        layout(location = 0) patch in vec2 vertices[4];

        layout(location = 4) patch in Cache {
            vec2 A;
            vec2 B;
        } cache;

        vec2 Bezier(float t,vec2 A, vec2 B, vec2 s)
        {
            return t * (t*A - B) + s;
        }
        void main() {
            float denom = gl_TessCoord.x + gl_TessCoord.z;
            float x = (denom > 0.0001) ? (gl_TessCoord.z / denom) : 0;
            float y = 1- gl_TessCoord.y;

            vec2 L = mix(vertices[0], vertices[1], y);
            vec2 R = mix(vertices[0], vertices[3], y);
            vec2 LR = mix(L,R, x);

            vec2 D = vertices[0];
            vec2 U = Bezier(x, cache.A, cache.B, vertices[1]);
            vec2 DU = mix(D,U,y);

            vec2 BU = mix(vertices[1],vertices[3], x);
            vec2 B = mix(vertices[0],BU, y);
            
            gl_Position =vec4(LR + DU - B, 0.5, 1.0);
        }
		";

    const string _source_fragment = @"
		#version 450

		layout(location = 0) out vec4 outColor;

		void main() {
			outColor = vec4(0, 1.0, 0, 1.0);
		}
		";
}
