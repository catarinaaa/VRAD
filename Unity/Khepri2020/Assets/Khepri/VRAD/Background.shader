Shader "Custom/Background" {
    SubShader {
        Pass {
            Stencil {
                Ref 1
                Comp Equal
            }
        }
    }
}
