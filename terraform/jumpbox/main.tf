# JumpBox

locals {
  name         = "devops"
  project_name = "devopskats"

  envs = [
    "develop",
    "master"
  ]
}

resource "aws_security_group" "jumpbox" {
  name   = "${local.name}-jumpbox"
  vpc_id = data.aws_vpc.default.id

  ingress {
    protocol    = "tcp"
    from_port   = 22
    to_port     = 22
    cidr_blocks = ["0.0.0.0/0"]
  }

  ingress {
    protocol    = "tcp"
    from_port   = 5901
    to_port     = 5901
    cidr_blocks = ["0.0.0.0/0"]
  }

  egress {
    protocol         = "-1"
    from_port        = 0
    to_port          = 0
    cidr_blocks      = ["0.0.0.0/0"]
    ipv6_cidr_blocks = ["::/0"]
  }
}

module "ec2_instance" {
  source  = "terraform-aws-modules/ec2-instance/aws"
  version = "~> 3.0"

  name = "${local.name}-jumpbox"

  ami                    = "ami-04505e74c0741db8d"
  instance_type          = "t2.small"
  key_name               = "culiops"
  monitoring             = false
  vpc_security_group_ids = [aws_security_group.jumpbox.id]
  subnet_id              = "subnet-f6c74afa"

  root_block_device = [
    {
      encrypted   = true
      volume_type = "gp3"
      throughput  = 200
      volume_size = 30
    },
  ]

  tags = {
    Env = "develop"
  }

  volume_tags = {
    Env = "develop"
  }
}

resource "aws_eip" "jumpbox" {
  instance = module.ec2_instance.id
  vpc      = true

  tags = {
    Env = "develop"
  }
}
