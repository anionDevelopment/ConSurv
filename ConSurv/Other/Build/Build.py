from ScriptCollection.TFCPS.Docker.TFCPS_CodeUnitSpecific_Docker import TFCPS_CodeUnitSpecific_Docker_Functions,TFCPS_CodeUnitSpecific_Docker_CLI

 
def build():
    tf:TFCPS_CodeUnitSpecific_Docker_Functions=TFCPS_CodeUnitSpecific_Docker_CLI.parse(__file__)
    tf.build(None,{
        "debian":tf._protected_sc.default_fallback_docker_registry,
    })
    tf.tfcps_Tools_General.merge_sbom_file_from_dependent_codeunit_into_this(tf.get_codeunit_folder(),tf.get_codeunit_name(),"ConSurvBackend",tf.use_cache())
    tf.tfcps_Tools_General.merge_sbom_file_from_dependent_codeunit_into_this(tf.get_codeunit_folder(),tf.get_codeunit_name(),"ConSurvFrontend",tf.use_cache())


if __name__ == "__main__":
    build()
