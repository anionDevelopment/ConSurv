from ScriptCollection.TFCPS.TFCPS_Generic import TFCPS_Generic_Functions, TFCPS_Generic_CLI
from ScriptCollection.OCIImages.OCIImageManager import OCIImageManager

def prepare_build_codeunits():
    t :TFCPS_Generic_Functions= TFCPS_Generic_CLI().parse(__file__)
    t.tfcps_Tools_General.ensure_certificate_authority_for_development_purposes_is_generated(t.repository_folder)
    t.tfcps_Tools_General.generate_certificate_for_development_purposes_for_product(t.repository_folder)
    t.tfcps_Tools_General.generate_tasksfile_from_workspace_file(t.repository_folder)
    t.tfcps_Tools_General.generate_codeunits_overview_diagram(t.repository_folder)
    t.tfcps_Tools_General.generate_svg_files_from_plantuml_files_for_repository(t.repository_folder,t.use_cache())
    t.sc.ensure_docker_network_is_available("consurv_net")
    ociImageManager:OCIImageManager=OCIImageManager(t.sc)
    t.tfcps_Tools_General.pull_images_of_test_services(t.repository_folder,None,{
        "mariadb":ociImageManager.get_registry_address_for_image(t.repository_folder,"MariaDB"),
        "postgres":ociImageManager.get_registry_address_for_image(t.repository_folder,"PostgreSQL"),
        "nginx":ociImageManager.get_registry_address_for_image(t.repository_folder,"nginx"),
        "adminer":ociImageManager.get_registry_address_for_image(t.repository_folder,"Adminer"),
    })
 

if __name__ == "__main__":
    prepare_build_codeunits()
