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
    logger.info("Get hour diff from ECS task created at: %s" % from_time)
    from_time = from_time.replace(tzinfo=None)
    now  = datetime.now()
    duration = now - from_time
    return duration.total_seconds()/60/60

def get_ecs_service(cluster_name, service_name):
    try:
        logger.info("Get ECS service info from cluster: %s and service: %s" % (cluster_name, service_name))
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

def get_ecs_tasks_by_service(cluster_name, service_name):
    try:
        logger.info("Get ECS tasks from cluster: %s and service: %s" % (cluster_name, service_name))
        response = ecs.list_tasks(
            cluster=cluster_name,
            serviceName=service_name
        )
        if not response['taskArns'] or len(response['taskArns']) < 1:
            logger.info("No tasks in cluster")
            return False
        return response['taskArns'] 
    except ClientError as error:
        logger.error(error)

def describe_ecs_tasks(cluster_name, tasks):
    try:
        logger.info("Found ECS tasks from cluster %s: %s" % (cluster_name, tasks))
        response = ecs.describe_tasks(
            cluster=cluster_name,
            tasks=tasks
        )

        if not response['tasks']:
            logger.info("No tasks in cluster")
            return False

        tasks_create_at = [item['createdAt'] for item in response['tasks']]
        return tasks_create_at[0]
    except ClientError as error:
        logger.error(error)

def update_ecs_service(cluster_name, service_name, remove_after_hours):
    try:
        ecs_tasks = get_ecs_tasks_by_service(cluster_name, service_name)
        if not ecs_tasks:
            logger.info("Can not find out the tasks from cluster: %s and service: %s" % (service_name, cluster_name))
            return False
        
        task_created_at = describe_ecs_tasks(cluster_name, ecs_tasks)
        if not task_created_at:
            logger.info("Can not find out the task created at %s" % task_created_at)
            return False
        
        task_created_hour_diff = get_hour_diff(task_created_at)
        if  task_created_hour_diff < int(remove_after_hours):
            logger.info("Not yet reach timeout of %sh(current: %s), skip to remove tasks" % (remove_after_hours, task_created_hour_diff))
            return False

        ecs.update_service(
            cluster=cluster_name,
            service=service_name,
            desiredCount=0
        )

        logger.info("Removed tasks after reached timeout of %sh(current: %s)" % (remove_after_hours, task_created_hour_diff))
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