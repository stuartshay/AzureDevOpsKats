node('docker') {

    stage('Git checkout') {
        git branch: 'master', credentialsId: 'gihub-key', url: 'git@github.com:stuartshay/AzureDevOpsKats.git'
    }

    stage('Docker build image') {
         sh '''mv docker/azuredevopskats-web-build.dockerfile/.dockerignore .dockerignore
         docker build -f docker/azuredevopskats-web-build.dockerfile/Dockerfile --build-arg BUILD_NUMBER=${BUILD_NUMBER} -t stuartshay/azuredevopskats:2.1.9-build .'''
         
         withCredentials([usernamePassword(credentialsId: 'docker-hub-navigatordatastore', usernameVariable: 'DOCKER_HUB_LOGIN', passwordVariable: 'DOCKER_HUB_PASSWORD')]) {
            sh "docker login -u ${DOCKER_HUB_LOGIN} -p ${DOCKER_HUB_PASSWORD}"
        }
        sh '''docker push stuartshay/azuredevopskats:2.1.9-build'''
    }

    stage ('Deploy Stack') {
        withCredentials([usernamePassword(credentialsId: 'JENKINS_ENV_KEY', passwordVariable: 'RANCHER_SECRET_KEY', usernameVariable: 'RANCHER_ACCESS_KEY')]) {
          sh 'rancher-compose --url https://rancher.navigatorglass.com/v2-beta/projects/1a5  -f docker/rancher/docker-compose.yml --project-name ImageGallery up imagegallery-api --force-upgrade -p -c -d'
        }
    }

   stage('Mail') {
        emailext attachLog: true, body: '', subject: "Jenkins build status - ${currentBuild.fullDisplayName}", to: 'sshay@yahoo.com'
    }

}
