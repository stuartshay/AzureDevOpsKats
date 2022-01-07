import os
import boto3
import jsonpickle
import logging
from botocore.exceptions import ClientError
from datetime import datetime

logger = logging.getLogger()
logger.setLevel(logging.INFO)
ecs = boto3.client('ecs', region_name="us-east-1")

def get_hour_diff(from_time):
    from_time = from_time.replace(tzinfo=None)
    now  = datetime.now()
    duration = now - from_time
    return duration.total_seconds()/60/60

def get_ecs_service(cluster_name, service_name):
    try:
        response = ecs.describe_services(
            cluster=cluster_name,
            services=[
                service_name,
            ]
        )

        if not response['services']:
            logger.info("No services in cluster")
            return False

        return response['services'][0]
        
    except ClientError as error:
        logger.error(error)
        
def update_ecs_service(cluster_name, service_name, remove_after_hours):
    try:
        ecs_service = get_ecs_service(cluster_name, service_name)

        if not ecs_service:
            logger.info("Can not find out the service %s from cluster %s" % (service_name, cluster_name))
            return False

        if  get_hour_diff(ecs_service['createdAt']) > remove_after_hours:
            logger.info("Not yet reach %sh(current: %s) of ECS service %s tasks from cluster %s" % (remove_after_hours, get_hour_diff(ecs_service['createdAt']), service_name, cluster_name))
            return False

        ecs.update_service(
            cluster=cluster_name,
            service=service_name,
            desiredCount=0
        )
    except ClientError as error:
        logger.error(error)

def lambda_handler(event, context):
    logger.info('## ENVIRONMENT VARIABLES\r' +
                jsonpickle.encode(dict(**os.environ)))
    logger.info('## EVENT\r' + jsonpickle.encode(event))
    logger.info('## CONTEXT\r' + jsonpickle.encode(context))

    REMOVE_AFTER_HOURS = os.environ.get('REMOVE_AFTER_HOURS', 6)
    ECS_CLUSTERS = os.environ.get('ECS_CLUSTERS', '')
    if not ECS_CLUSTERS:
        logger.error("No ECS cluster in OS ENV: %s" % ECS_CLUSTERS)

    ecs_clusters = [item.strip()
                       for item in ECS_CLUSTERS.split(",")]
                       
    for cluster in ecs_clusters:
        update_ecs_service(cluster, cluster, int(REMOVE_AFTER_HOURS))