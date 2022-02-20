
# Create virtual environments for python with conda

Update Conada
```
conda -V
conda update conda
```

Create and Activate virtual environment
```
conda create -n my_env python=3.8.8 anaconda
source activate my_env
```

Install additional Python packages to a virtual environment.
```bash
conda install -n [environment] [package]
```
## Add Ansible Packages

```
 conda install -c conda-forge ansible
 conda install -c conda-forge boto3
```

