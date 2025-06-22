pipeline {
    agent any  // Run the pipeline on any available Jenkins agent

    environment {
        // Use stored Jenkins credentials to authenticate with Docker Hub
        DOCKER_HUB_CREDENTIALS = credentials('docker-hub-credential') 

        // Define the Docker image name to be built and pushed
        DOCKER_IMAGE = 'gameservice/mydotnetapp'
    }

    stages {

        
        stage('Resolve') {
            steps {
                sh 'export DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=1'
            }
        }


        stage('Verify .NET') {
            steps {
                sh 'dotnet --version'
            }
        }
        stage('Build') {
            steps {
                // Build the .NET project in Release mode without restoring packages
                sh 'dotnet build '
            }
        }

        stage('Test') {
            steps {
                // Run unit tests with normal verbosity
                sh 'dotnet test '
            }
        }

        stage('Publish') {
            steps {
                // Publish the application (generate deployable binaries)
                sh 'dotnet publish'
            }
        }

        stage('Docker Build') {
            steps {
                // Build a Docker image using the Dockerfile in the project root
                sh 'docker build -t $DOCKER_IMAGE:latest .'
            }
        }

         stage('login to dockerhub') {
            steps {
                sh 'echo $DOCKERHUB_CREDENTIALS_PSW | docker login -u $DOCKERHUB_CREDENTIALS_USR --password-stdin'
            }
        }

        stage('Docker Push') {
            steps {
                // Authenticate to Docker Hub and push the latest image
                withDockerRegistry(credentialsId: 'docker-hub-credential', url: '') {
                    sh 'docker push $DOCKER_IMAGE:latest'
                }
            }
        }
    }

    post {
        always {
            // Clean up workspace after the build, whether successful or failed
            cleanWs()
        }
    }
}
